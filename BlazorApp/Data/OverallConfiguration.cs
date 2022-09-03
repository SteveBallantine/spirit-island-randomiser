using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using SiRandomizer.Extensions;

namespace SiRandomizer.Data
{
    public class OverallConfiguration : IValidatableObject
    {
        public OptionGroup<Adversary> Adversaries { get; set; }
        public OptionGroup<Board> Boards { get; set; }
        public OptionGroup<Map> Maps { get; set; }
        public OptionGroup<Expansion> Expansions { get; set; }
        public OptionGroup<Spirit> Spirits { get; set; }
        public OptionGroup<Scenario> Scenarios { get; set; }

        [Required]
        [Range(1, 6, ErrorMessage = "Number of players must be 1 - 6")]
        public int Players {get;set;}
        [Required]
        [Range(0, 20, ErrorMessage = "Minimum difficulty must be 0 - 20")]
        public int MinDifficulty {get;set;}
        [Required]
        [Range(0, 20, ErrorMessage = "Maximum difficulty must be 0 - 20")]
        public int MaxDifficulty {get;set;}

        [Range(0, 100, ErrorMessage = "Additional board chance must be 0 - 100")]
        public int AdditionalBoardChance {get;set;} = 0;
        [Range(0, 100, ErrorMessage = "Combined adversaries chance must be 0 - 100")]
        public int CombinedAdversariesChance {get;set;} = 0;
        
        // The internal getters on these legacy properties mean that they will not be serialized to json
        // when storing the configuration.
        // However, the setter is public, so old configs can still be deserialized and migrated using the code in TakeSettingsFrom.
        public OptionChoice? AdditionalBoard  {internal get;set;} = null;
        public OptionChoice? CombinedAdversaries {internal get;set;} = null;

        public OptionChoice RandomThematicBoards {get;set;} = OptionChoice.Block;
        public OptionChoice ImbalancedArcadeBoards {get;set;} = OptionChoice.Block;
        public OptionChoice Aspects {get;set;} = OptionChoice.Allow;

        [JsonIgnore]
        public int MaxAdditionalBoards 
        {
            get 
            {
                return AdditionalBoardChance > 0 ? 1 : 0;
            }
        }

        [JsonIgnore]
        public int MinAdditionalBoards 
        {
            get
            {
                return AdditionalBoardChance == 100 ? 1 : 0;
            }
        }

        public OverallConfiguration() {}         

        /// <summary>
        /// Update the user-configurable settings on this configuration object
        /// with those from the supplied configuration object.
        /// </summary>
        /// <param name="other"></param>
        public void TakeSettingsFrom(OverallConfiguration other)
        {            
            this.AdditionalBoardChance = other.AdditionalBoardChance;
            this.CombinedAdversariesChance = other.CombinedAdversariesChance;

            // In future, AdditionalBoard and CombinedAdversaries will always be null.
            // If the user has a different value, then migrate it by setting the corresponding 'Chance' value instead.
            if(other.AdditionalBoard.HasValue)
            {
                switch (other.AdditionalBoard)
                {
                    case OptionChoice.Force:
                        this.AdditionalBoardChance = 100;
                        break;
                    case OptionChoice.Block:
                        this.AdditionalBoardChance = 0;
                        break;
                    case OptionChoice.Allow:
                        this.AdditionalBoardChance = 50;
                        break;
                    default:
                        break;
                }
            }            
            if(other.CombinedAdversaries.HasValue)
            {
                switch (other.CombinedAdversaries)
                {
                    case OptionChoice.Force:
                        this.CombinedAdversariesChance = 100;
                        break;
                    case OptionChoice.Block:
                        this.CombinedAdversariesChance = 0;
                        break;
                    case OptionChoice.Allow:
                        this.AdditionalBoardChance = 50;
                        break;
                    default:
                        break;
                }
            }
            
            this.MaxDifficulty = other.MaxDifficulty;
            this.MinDifficulty = other.MinDifficulty;
            this.Players = other.Players;
            this.RandomThematicBoards = other.RandomThematicBoards;
            this.ImbalancedArcadeBoards = other.ImbalancedArcadeBoards;
            this.Aspects = other.Aspects;

            TakeSettingsFrom(Expansions, other.Expansions);
            // Adversaries is a collection (of AdversaryLevels) so need to specify
            // the child type in order for this to work properly.
            // This is all a bit clunky, but gets the job done.
            TakeSettingsFrom<Adversary, AdversaryLevel>(Adversaries, other.Adversaries);
            TakeSettingsFrom(Boards, other.Boards);
            TakeSettingsFrom(Maps, other.Maps);
            TakeSettingsFrom(Scenarios, other.Scenarios);
            TakeSettingsFrom<Spirit, SpiritAspect>(Spirits, other.Spirits, SpiritFactory);
        }

        private void TakeSettingsFrom<TItem>(
            IComponentCollection<TItem> destination, 
            IComponentCollection<TItem> source)
            where TItem : INamedComponent
        {
            // We call the alternative signature for the method, passing the 
            // item type as the child type.
            // The checks in the other method will handle whether TItem is
            // actually a collection class or not.
            TakeSettingsFrom<TItem, TItem>(destination, source);
        }

        private void TakeSettingsFrom<TItem, TChild>(
            IComponentCollection<TItem> destination, 
            IComponentCollection<TItem> source,
            Func<TItem, TItem> itemFactory = null)
            where TItem : INamedComponent
            where TChild : INamedComponent
        {
            if(source != null)
            {
                var itemsAreCollections = typeof(TItem).IsNamedComponentCollection();

                destination.Selected = source.Selected;
                foreach(var sourceItem in source)
                {         
                    if(destination.HasChild(sourceItem.Name) == false)
                    {
                        if(itemFactory == null) 
                        {
                            throw new Exception($"No item factory specified for '{typeof(TItem).Name}'");
                        }
                        // The default list does not include this item, so create and add it.
                        destination.Add(itemFactory(sourceItem));
                    }
                    else 
                    {
                        // The default list does include this item so get the 'selected' status.
                        var destinationItem = destination.Single(i => i.Name == sourceItem.Name);       
                        destinationItem.Selected = sourceItem.Selected;
                        if(itemsAreCollections)
                        {
                            // Repeat for any children.
                            TakeSettingsFrom(
                                destinationItem as IComponentCollection<TChild>, 
                                sourceItem as IComponentCollection<TChild>);
                        }
                    }
                }

                // Remove any items that are in the source but not the destination.
                // (e.g. this can happen if you have a preset (A) with homebrew spirits and another (B) without.
                // when you switch from A to B, we need to remove those additional spirits.)
                var notInSource = destination.Where(i => source.Any(j => j.Name == i.Name) == false);
                foreach(var item in notInSource)
                {
                    destination.Remove(item.Name);
                }
            }
        }

        /// <summary>
        /// Duplicates a spirit using this configuation object as the new instance's parent.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private Spirit SpiritFactory(Spirit source)
        {
            var spirit = new Spirit(source.Name, this, Spirits, Expansions[Expansion.Homebrew], source.BaseComplexity);
            spirit.Deletable = true;
            foreach(var aspect in spirit) 
            {
                if(spirit.HasChild(aspect.Name) == false) 
                {
                    var newAspect = new SpiritAspect(aspect.Name, this, spirit, Expansions[Expansion.Homebrew]);
                    aspect.Deletable = true;
                    spirit.Add(newAspect);
                }
            }
            return spirit;
        }

        public string ScenariosPanelClass = "";
        public string SpiritsPanelClass = "";
        public string MapsPanelClass = "";
        public string AdversariesPanelClass = "";
        public string BoardsPanelClass = "";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ScenariosPanelClass = "";
            SpiritsPanelClass = "";
            MapsPanelClass = "";
            AdversariesPanelClass = "";
            BoardsPanelClass = "";

            if(MaxDifficulty < MinDifficulty)
            {
                yield return new ValidationResult("Maximum difficulty must be geater than or equal to minimum difficulty",
                    new[] { nameof(MaxDifficulty) });
            }
            if(Players + MaxAdditionalBoards > Boards.Count(b => b.Selected))
            {
                BoardsPanelClass = "panel-invalid";
                yield return new ValidationResult("Player count + max additional boards must be no bigger than the number of selected boards",
                    new[] { nameof(Boards) });
            }            
            if(Scenarios.All(s => s.Selected == false))
            {
                ScenariosPanelClass = "panel-invalid";
                yield return new ValidationResult("Must pick at least one scenario", new[] { nameof(Scenarios) });
            }
            if(Adversaries.SelectMany(a => a.Levels).All(s => s.Selected == false))
            {
                AdversariesPanelClass = "panel-invalid";
                yield return new ValidationResult("Must pick at least one adversary", new[] { nameof(Adversaries) });
            }
            if(Maps.All(s => s.Selected == false))
            {
                MapsPanelClass = "panel-invalid";
                yield return new ValidationResult("Must pick at least one maps", new[] { nameof(Maps) });
            }
            if(Spirits.Count(s => s.Selected) < Players)
            {
                SpiritsPanelClass = "panel-invalid";
                yield return new ValidationResult($"Must pick at least {Players} spirits for {Players} players", new[] { nameof(Spirits) });
            }            
        }
    }
}
