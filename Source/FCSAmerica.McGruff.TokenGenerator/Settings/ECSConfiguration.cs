using System.Collections.Generic;

namespace FCSAmerica.McGruff.TokenGenerator.Settings
{
    public class EcsConfiguration
    {
        public List<ConfigurationListItem> ConfigurationList;
        public List<string> GlobalAppList;
    }

    public class ConfigurationListItem
    {
        public string PartnerId { get; set; }
        public List<EcsSetting> ConfigurationSettings { get; set; }
    }

    public class EcsSetting
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the configuration type.
        /// </summary>
        public int ConfigurationType { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// Gets the Computed Config Value  
        /// </summary>
        public string ComputedConfigValue { get; set; }
    }
}