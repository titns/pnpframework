﻿using Microsoft.SharePoint.Client;
using PnP.Framework.Provisioning.Model;
using System.Collections.Generic;

namespace PnP.Framework.Provisioning.ObjectHandlers
{
    /// <summary>
    /// This class cleans up the template by removing artifacts that are not supported in modern sites.
    /// </summary>
    internal class NoScriptTemplateCleaner
    {
        public ProvisioningMessagesDelegate MessagesDelegate { get; set; }

        private readonly Web _web;

        /// <summary>
        /// Creates a new instance of the template cleaner.
        /// </summary>
        /// <param name="web">The web to check if it is a modern site</param>
        public NoScriptTemplateCleaner(Web web)
        {
            _web = web;
        }

        /// <summary>
        /// Removes artifacts that are not supported by modern sites.
        /// </summary>
        /// <param name="template">The template to clean</param>
        /// <returns></returns>
        public ProvisioningTemplate CleanUpBeforeProvisioning(ProvisioningTemplate template)
        {
            bool isNoScriptSite = _web.IsNoScriptSite();

            var listsToRemove = new List<ListInstance>();

            foreach (var templateList in template.Lists)
            {
                if (isNoScriptSite && templateList.Url == "Style Library")
                {
                    listsToRemove.Add(templateList);
                    WriteMessage(string.Format(CoreResources.Provisioning_ObjectHandlers_ListInstances_List__0__is_Style_Library_of_NoScript_will_Skip, templateList.Title), ProvisioningMessageType.Warning);
                }
            }
            while (listsToRemove.Count > 0)
            {
                var listToRemove = listsToRemove[0];
                template.Lists.Remove(listToRemove);
                listsToRemove.RemoveAt(0);
            }

            return template;
        }

        internal void WriteMessage(string message, ProvisioningMessageType messageType)
        {
            if (MessagesDelegate != null)
            {
                MessagesDelegate(message, messageType);
            }
        }
    }
}
