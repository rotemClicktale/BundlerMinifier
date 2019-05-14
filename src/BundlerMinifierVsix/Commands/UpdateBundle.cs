﻿using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace BundlerMinifierVsix.Commands
{
    internal sealed class UpdateBundle
    {
        private readonly Package _package;

        private UpdateBundle(Package package)
        {
            _package = package;

            var commandService = (OleMenuCommandService)ServiceProvider.GetService(typeof(IMenuCommandService));
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.guidBundlerCmdSet, PackageIds.UpdateBundle);
                var menuItem = new OleMenuCommand(UpdateSelectedBundle, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var files = ProjectHelpers.GetSelectedItemPaths();
            button.Visible = false;

            int count = files.Count();

            if (count == 0) // Project
            {
                var project = ProjectHelpers.GetActiveProject();

                if (project == null)
                    return;

                string config = project.GetConfigFile();

                if (!string.IsNullOrEmpty(config) && File.Exists(config))
                {
                    button.Visible = true;
                    button.Enabled = button.Visible && BundleService.IsOutputProduced(config);
                }
            }
            else
            {
                button.Visible = files.Count() == 1 && Path.GetFileName(files.First()) == Constants.CONFIG_FILENAME;
                button.Enabled = button.Visible && BundleService.IsOutputProduced(files.First());
            }
        }

        public static UpdateBundle Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new UpdateBundle(package);
        }

        private void UpdateSelectedBundle(object sender, EventArgs e)
        {
            try
            {
                var file = ProjectHelpers.GetSelectedItemPaths().FirstOrDefault();

                if (string.IsNullOrEmpty(file)) // Project
                {
                    var project = ProjectHelpers.GetActiveProject();

                    if (project != null)
                        file = project.GetConfigFile();
                }

                if (!string.IsNullOrEmpty(file))
                {
                    BundleService.Process(file);
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
         
        }
    }
}
