using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;

namespace SiRandomizer.Services
{
    public class PresetService
    {
        private const string OldStorageName = "si-randomizer-config";
        private const string StorageNamePresets = "si-randomizer-presets";
        private const string StorageNamePresetPrefix = "si-randomizer-config-";
        private const string DefaultPreset = "Default";

        private IJSRuntime _jsRuntime;
        private ConfigurationService _configService;
        private ILogger<PresetService> _logger;

        public Presets Presets { get; private set; }

        public event EventHandler RefreshRequired;

        public PresetService(IJSRuntime jsRuntime,
            ConfigurationService configService,
            ILogger<PresetService> logger)
        {
            _jsRuntime = jsRuntime;
            _configService = configService;
            _logger = logger;
        }

        /// <summary>
        /// Save the current configuration to the current preset.
        /// </summary>
        /// <returns></returns>
        public async Task SaveCurrentConfigurationAsync() 
        {
            if(Presets == null)
            {
                throw new Exception("Not initialised yet");
            }

            await SavePresetAsync(Presets.Current);
        }
        
        /// <summary>
        /// Add the specified preset to those that are available and make it the current preset.
        /// It will either be unconfigured or can optionally copy the configuration from the 
        /// current preset.
        /// </summary>
        /// <param name="preset"></param>
        public async Task AddPresetAsync(string preset, bool populateFromCurrent)
        {
            if(Presets == null)
            {
                throw new Exception("Not initialised yet");
            }

            if(Presets.Available.Contains(preset))
            {
                throw new ArgumentException($"A preset called '{preset}' already exists");
            }
            Presets.Available.Add(preset);

            if(populateFromCurrent)
            {
                // Save the current configuration to the new preset
                await SavePresetAsync(preset);
            }

            // Change the current preset to the new one
            Presets.Current = preset;
        }

        /// <summary>
        /// Remove the specified preset and its associated configuration.
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        public async Task RemovePresetAsync(string preset)
        {
            if(Presets == null)
            {
                throw new Exception("Not initialised yet");
            }

            if(Presets.Available.Count == 1)
            {
                throw new ArgumentException($"Unable to delete the last preset. There must always be at least 1", nameof(preset));
            }
            if(Presets.Available.Contains(preset) == false)
            {
                throw new ArgumentException($"'{preset}' is not an available preset", nameof(preset));
            }
            // If we're deleting the current preset then move to the first one in the list
            // that is not the one we're deleting.
            if(Presets.Current == preset)
            {                
                Presets.Current = Presets.Available.First(p => p != preset);
            }
            // Remove the preset from the list and save the changes.
            Presets.Available.Remove(preset);
            await SavePresetsAsync();
            // Delete the assocaited configuration
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageNamePresetPrefix + preset);
        }

        /// <summary>
        /// Load the available presets and current preset from storage.
        /// </summary>
        /// <returns></returns>
        public async Task InitialisePresetsAsync()
        {
            if(Presets != null)
            {
                throw new Exception("Already initialised");
            }

            Presets result = null;
            try
            {
                var jsonConfig = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageNamePresets);
                if(string.IsNullOrEmpty(jsonConfig) == false)
                {
                    result = JsonSerializer.Deserialize<Presets>(jsonConfig);
                }
            } catch { /* Ignore the error and create a new presets config */ }

            if(result == null)
            {
                result = new Presets()
                {
                    Current = DefaultPreset,
                    Available = new List<string>() { DefaultPreset } 
                };
            }
            Presets = result;
            Presets.PropertyChanging += PresetsChangingAsync;
            Presets.PropertyChanged += PresetsChangedAsync;

            // Load the configuration for the current preset
            var savedConfig = await LoadPresetAsync();
            _configService.Current.TakeSettingsFrom(savedConfig, _logger);
        }

        public async void PresetsChangingAsync(object sender, PropertyChangedEventArgs args) 
        {
            // If the current preset is changing then save the current configuration before it's updated.
            if(args.PropertyName == nameof(Presets.Current)) 
            {                
                // Save the current configuration
                if(string.IsNullOrEmpty(Presets.Current) == false)
                {
                    await SavePresetAsync(Presets.Current);
                }
            }
        }

        public async void PresetsChangedAsync(object sender, PropertyChangedEventArgs args) 
        {
            // If the current preset has changed then update the configuration to reflect this.
            if(args.PropertyName == nameof(Presets.Current)) 
            {
                // Save the name of the preset as the current one
                await SavePresetsAsync();
                // Load the configuration for the preset
                var savedConfig = await LoadPresetAsync();
                _configService.Current.TakeSettingsFrom(savedConfig, _logger);
                OnRefreshRequired();
            }
        }

        private void OnRefreshRequired()
        {
            if(RefreshRequired != null) 
            {
                RefreshRequired.Invoke(this, null);
            }
        }

        /// <summary>
        /// Load the current preset and return the configuration object.
        /// </summary>
        /// <returns></returns>
        private async Task<OverallConfiguration> LoadPresetAsync()
        {
            try
            {
                // Try to get configuration from local storage
                var jsonConfig = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageNamePresetPrefix + Presets.Current);
                // If there is no config and we're trying to load the default preset
                // then check the old storage name as well.
                if(string.IsNullOrEmpty(jsonConfig) && 
                    Presets.Current == DefaultPreset) 
                {
                    jsonConfig = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", OldStorageName);
                }
                if(string.IsNullOrEmpty(jsonConfig) == false)
                {
                    return JsonSerializer.Deserialize<OverallConfiguration>(jsonConfig);
                }
            }
            catch (Exception ex)
            {
                throw new SiException($"Failed to load preset '{Presets.Current}'", ex);
            }

            // If we haven't found anything the return a new configuration
            return _configService.CreateConfiguration();
        }

        /// <summary>
        /// Save the current configuration to the current preset.
        /// </summary>
        /// <returns></returns>
        private async Task SavePresetAsync(string presetName)
        {
            var jsonConfig = JsonSerializer.Serialize(_configService.Current);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageNamePresetPrefix + presetName, jsonConfig);            
        }

        /// <summary>
        /// Save the available preset names as well as the name of the current preset.
        /// </summary>
        /// <returns></returns>
        private async Task SavePresetsAsync()
        {
            var jsonPresets = JsonSerializer.Serialize(Presets);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageNamePresets, jsonPresets);       
        }
    }
}