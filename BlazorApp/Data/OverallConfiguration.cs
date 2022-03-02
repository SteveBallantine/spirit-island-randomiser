using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public OptionChoice AdditionalBoard {get;set;} = OptionChoice.Block;
        public OptionChoice CombinedAdversaries {get;set;} = OptionChoice.Block;
        public OptionChoice RandomThematicBoards {get;set;} = OptionChoice.Block;

        [JsonIgnore]
        public int MaxAdditionalBoards 
        {
            get 
            {
                switch (AdditionalBoard)
                {
                    case OptionChoice.Allow:
                    case OptionChoice.Force:
                        return 1;
                    case OptionChoice.Block:
                        return 0;
                    default:
                        throw new Exception("Unexpected value");
                }
            }
        }

        [JsonIgnore]
        public int MinAdditionalBoards 
        {
            get
            {
                return AdditionalBoard == OptionChoice.Force ? 1 : 0;
            }
        }

        public OverallConfiguration() {}

        public OverallConfiguration(
            OptionGroup<Adversary> adversaries,
            OptionGroup<Board> boards,
            OptionGroup<Map> maps,
            OptionGroup<Expansion> expansions,
            OptionGroup<Spirit> spirits,
            OptionGroup<Scenario> scenarios) 
        {
            Adversaries = adversaries;
            Boards = boards;
            Maps = maps;
            Expansions = expansions;
            Spirits = spirits;
            Scenarios = scenarios;
        }

        /// <summary>
        /// Update the user-configurable settings on this configuration object
        /// with those from the supplied configuration object.
        /// </summary>
        /// <param name="other"></param>
        public void TakeSettingsFrom(OverallConfiguration other)
        {
            this.AdditionalBoard = other.AdditionalBoard;
            this.CombinedAdversaries = other.CombinedAdversaries;
            this.MaxDifficulty = other.MaxDifficulty;
            this.MinDifficulty = other.MinDifficulty;
            this.Players = other.Players;
            this.RandomThematicBoards = other.RandomThematicBoards;

            TakeSettingsFrom(Expansions, other.Expansions);
            // Adversaries is a collection (of AdversaryLevels) so need to specify
            // the child type in order for this to work properly.
            // This is all a bit clunky, but gets the job done.
            TakeSettingsFrom<Adversary, AdversaryLevel>(Adversaries, other.Adversaries);
            TakeSettingsFrom(Boards, other.Boards);
            TakeSettingsFrom(Maps, other.Maps);
            TakeSettingsFrom(Scenarios, other.Scenarios);
            TakeSettingsFrom(Spirits, other.Spirits);
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
            IComponentCollection<TItem> source)
            where TItem : INamedComponent
            where TChild : INamedComponent
        {
            if(source != null)
            {
                var itemsAreCollections = typeof(TItem).IsNamedComponentCollection();

                destination.Selected = source.Selected;
                foreach(var sourceItem in source)
                {         
                    var destinationItem = destination.Single(i => i.Name == sourceItem.Name);       
                    destinationItem.Selected = sourceItem.Selected;
                    if(itemsAreCollections)
                    {
                        TakeSettingsFrom(
                            destinationItem as IComponentCollection<TChild>, 
                            sourceItem as IComponentCollection<TChild>);
                    }
                }
            }
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
