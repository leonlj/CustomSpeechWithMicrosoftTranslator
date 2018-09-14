using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MicrosoftSpeechSDKSamples
{
    public sealed class Setting
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// Gets or sets Standard Subscription Key
        /// </summary>
        public string StandardSubscriptionKey
        {
            get
            {
                return standardSubscriptionKey;
            }
            set
            {
                standardSubscriptionKey = value?.Trim();
                SaveLocalKey("StandardSubscriptionKey",standardSubscriptionKey);
            }
        }
        private string standardSubscriptionKey;

        /// <summary>
        /// Gets or sets region name of the standard service
        /// </summary>
        public string StandardRegion
        {
            get
            {
                return standardRegion;
            }
            set
            {
                standardRegion = value;
                SaveLocalKey("StandardRegion", standardRegion);
            }
        }
        private string standardRegion;

        /// <summary>
        /// Gets or sets recognition language of the standard service
        /// </summary>
        public string StandardRecognitionLanguage
        {
            get
            {
                return standardRecongnitionLanguage;
            }
            set
            {
                standardRecongnitionLanguage = value;
                SaveLocalKey("RecongnitionLanguage", standardRecongnitionLanguage);
            }
        }
        private string standardRecongnitionLanguage;

        /// <summary>
        /// Gets or sets custom subscription key
        /// </summary>
        public string CustomSubscriptionKey
        {
            get
            {
                return customSubscriptionKey;
            }
            set
            {
                customSubscriptionKey = value?.Trim();
                SaveLocalKey("CustomSubscriptionKey", customSubscriptionKey);
            }
        }
        private string customSubscriptionKey;

        /// <summary>
        /// Gets or sets region name of the custom service
        /// </summary>
        public string CustomRegion
        {
            get
            {
                return customRegion;
            }
            set
            {
                customRegion = value;
                SaveLocalKey("CustomRegion", customRegion);
            }
        }
        private string customRegion;

        /// <summary>
        /// Gets or sets recognition language of the custom service
        /// </summary>
        public string CustomRecognitionLanguage
        {
            get
            {
                return customRecognitionLanguage;
            }
            set
            {
                customRecognitionLanguage = value;
                SaveLocalKey("CustomRecognitionLanguage", customRecognitionLanguage);
            }
        }
        private string customRecognitionLanguage;

        /// <summary>
        /// Gets or sets deploymentID of the custom server
        /// </summary>
        public string CustomDeploymentID
        {
            get
            {
                return customDeploymentID;
            }
            set
            {
                customDeploymentID = value?.Trim();
                SaveLocalKey("CustomDeploymentID",customDeploymentID);
            }
        }
        private string customDeploymentID;

        /// <summary>
        /// Gets or sets Subscription Key
        /// </summary>
        public string TranslateSubscriptionKey
        {
            get
            {
                return translateSubscriptionKey;
            }
            set
            {
                translateSubscriptionKey = value?.Trim();
                SaveLocalKey("TranslateSubscriptionKey", translateSubscriptionKey);
            }
        }
        private string translateSubscriptionKey;

        /// <summary>
        /// Gets or sets region name of the translator service
        /// </summary>
        public string TranslateRegion
        {
            get
            {
                return translateRegion;
            }
            set
            {
                translateRegion = value;
                SaveLocalKey("TranslateRegion", translateRegion);
            }
        }
        private string translateRegion;

        /// <summary>
        /// Gets or sets translate from language
        /// </summary>
        public string TranslateFromLanguage
        {
            get
            {
                return translateFromLanguage;
            }
            set
            {
                translateFromLanguage = value;
                SaveLocalKey("TranslateFromLanguage", translateFromLanguage);
            }
        }
        private string translateFromLanguage;

        /// <summary>
        /// Gets or sets translate to language
        /// </summary>
        public string TranslateToLanguage
        {
            get
            {
                return translateToLanguage;
            }
            set
            {
                translateToLanguage = value;
                SaveLocalKey("TranslateToLanguage",translateToLanguage);
            }
        }
        private string translateToLanguage;

        public string SpeechServerType
        {
            get
            {
                return speechServerType;
            }
            set
            {
                speechServerType = value;
                SaveLocalKey("SpeechServerType", speechServerType);
            }
        }
        private string speechServerType;


        public Setting()
        {
            this.standardRecongnitionLanguage = ReadLocalKey("StandardRecongnitionLanguage");
            this.standardRegion = ReadLocalKey("StandardRegion");
            this.standardSubscriptionKey = ReadLocalKey("StandardSubscriptionKey");
            this.customDeploymentID = ReadLocalKey("CustomDeploymentID");
            this.customRecognitionLanguage = ReadLocalKey("CustomRecognitionLanguage");
            this.customRegion = ReadLocalKey("CustomRegion");
            this.customSubscriptionKey = ReadLocalKey("CustomSubscriptionKey");
            this.translateFromLanguage = ReadLocalKey("TranslateFromLanguage");
            this.translateToLanguage = ReadLocalKey("TranslateToLanguage");
            this.translateRegion = ReadLocalKey("TranslateRegion");
            this.translateSubscriptionKey = ReadLocalKey("TranslateSubscriptionKey");
            this.speechServerType = ReadLocalKey("SpeechServerType");
        }

        private void SaveLocalKey(string key, string value)
        {
            localSettings.Values[key] = value;
        }

        private string ReadLocalKey(string key)
        {
            var ls = localSettings.Values[key];
            if (ls != null)
                return ls.ToString();
            else
                return string.Empty;
        }
    }
}
