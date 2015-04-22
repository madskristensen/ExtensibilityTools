using System;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    /// <summary>
    /// Basic class to wrap code about executed menu command.
    /// </summary>
    abstract class BaseCommand
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DTE2 _dte;

        protected BaseCommand(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;
            _dte = serviceProvider.GetService(typeof(SDTE)) as DTE2;

            SetupCommands();
        }

        /// <summary>
        /// Setups new menu command with handlers.
        /// </summary>
        protected OleMenuCommand AddCommand(Guid menuGroup, int commandID, EventHandler invokeHandler, EventHandler beforeQueryHandler)
        {
            if (invokeHandler == null)
                throw new ArgumentNullException("invokeHandler", "Missing action to perform");

            OleMenuCommandService commandService = GetService<OleMenuCommandService, IMenuCommandService>();
            if (commandService != null)
            {
                OleMenuCommand addCustomToolItem = new OleMenuCommand(invokeHandler, new CommandID(menuGroup, commandID));

                if (beforeQueryHandler != null)
                {
                    addCustomToolItem.BeforeQueryStatus += beforeQueryHandler;
                }

                commandService.AddCommand(addCustomToolItem);
            }

            return null;
        }

        /// <summary>
        /// Gets the currently selected solution item.
        /// </summary>
        protected UIHierarchyItem GetSelectedItem()
        {
            var items = (Array)_dte.ToolWindows.SolutionExplorer.SelectedItems;

            foreach (UIHierarchyItem selItem in items)
            {
                return selItem;
            }

            return null;
        }

        /// <summary>
        /// Gets the specific service.
        /// </summary>
        protected T GetService<T, S>() where T : class
        {
            return _serviceProvider.GetService(typeof(S)) as T;
        }

        /// <summary>
        /// Gets the specific service.
        /// </summary>
        protected T GetService<T>() where T : class
        {
            return _serviceProvider.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Overriden by child class to setup own menu commands and bind with invocation handlers.
        /// </summary>
        protected abstract void SetupCommands();
    }
}
