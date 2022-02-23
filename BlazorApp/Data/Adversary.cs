using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    [JsonConverter(typeof(AdversaryConverter))]
    public class Adversary : SelectableComponentBase<Adversary>, IComponentCollection<AdversaryLevel>, IExpansionContent
    {
        public const string NoAdversary = "No Adversary";
        public const string England = "England";
        public const string BrandenburgPrussia = "Brandenburg Prussia";
        public const string Sweden = "Sweden";
        public const string France = "France";
        public const string Habsburg = "Habsburg";
        public const string Russia = "Russia";
        public const string Scotland = "Scotland";

        [JsonIgnore]
        public Expansion Expansion { get; set; }

        private List<AdversaryLevel> _levels = new List<AdversaryLevel>();
        /// <summary>
        /// Don't need this to be public, as the class implements 
        /// IComponentCollection and exposes the levels as an IEnumerable
        /// in order to simplify the nested lists implementation in the UI.
        /// 
        /// However, it can be a little confusing working with it that
        /// way in code, so we also add this accessor for clarity.
        /// For example:
        /// `Adversary.England.Where(a => a.Selected)`
        /// is the same as
        /// `Adversary.England.Levels.Where(a => a.Selected)`
        /// But the second approach makes the intent clearer to the reader.
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public IReadOnlyList<AdversaryLevel> Levels => _levels;

        public Adversary() {}

        public Adversary(
            string name, 
            Expansion expansion)
            : base (name)
        {   
            Expansion = expansion;
            PropertyChanged += ThisUpdated;
        }

        public void Add(AdversaryLevel level)
        {
            level.Adversary = this;
            level.PropertyChanged += AdversaryLevelUpdated;
            _levels.Add(level);
        }

        public IEnumerator<AdversaryLevel> GetEnumerator()
        {
            return Levels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Levels.GetEnumerator();
        }

        public void AdversaryLevelUpdated (object sender, PropertyChangedEventArgs args) {
            // When child is updated, trigger the property changed
            // event on the group.
            OnPropertyChanged("Level_" + args.PropertyName);
        }

        public void ThisUpdated (object sender, PropertyChangedEventArgs args) {
            // When the selection state of this adversary is changed, 
            // update all it's levels accordingly.
            if(args.PropertyName == nameof(Selected) &&
                Levels != null && Levels.Count > 0) 
            {
                foreach(var level in Levels) 
                {
                    level.Selected = Selected;
                }
            }
        }
    }
}
