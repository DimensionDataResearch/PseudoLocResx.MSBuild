// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PseudoLocResxTask.cs">
//   Copyright belongs to Manish Kumar
// </copyright>
// <summary>
//   Build task to convert Resource file to pesudo loc resx file
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoLocResx.MSBuild
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel.Design;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Resources;
	using System.Web.Script.Serialization;

	using Microsoft.Build.Framework;

	/// <summary>
    /// Build task to convert Resource file to Java script Object Notation file
    /// </summary>
    public class PseudoLocResxTask : ITask
	{
		/// <summary>
		/// The pseudo-loc locale.
		/// </summary>
		public const string locale = "qps-ploc";

        /// <summary>
        /// Gets or sets Build Engine
        /// </summary>
        public IBuildEngine BuildEngine { get; set; }

        /// <summary>
        /// Gets or sets Host Object
        /// </summary>
        public ITaskHost HostObject { get; set; }

        /// <summary>
        /// Gets or sets list of EmbeddedResource Files
        /// </summary>
        [Required]
        public ITaskItem[] EmbeddedResourcesItems { get; set; }

        /// <summary>
        /// Gets or sets Project Full Path
        /// </summary>
        [Required]
        public string ProjectFullPath { get; set; }

        /// <summary>
        /// Gets or sets Project Output Path
        /// </summary>
        [Required]
        public string OutputPath { get; set; }

        /// <summary>
        /// Executes the Task
        /// </summary>
        /// <returns>True if success</returns>
        public bool Execute()
        {
            if (!this.EmbeddedResourcesItems.Any())
            {
                this.BuildEngine.LogMessageEvent(
                    new BuildMessageEventArgs(
                        string.Format(
							"Skipping conversion of Resource files to Pseudo-Loc, as there are no resource files found in the project. If your resx file is not being picked up, check if the file is marked for build action = 'Embedded Resource'"),
                        string.Empty,
						"PseudoLocResx",
                        MessageImportance.Normal));
            }

            var args = new BuildMessageEventArgs(
                "Started converting Resx To Pseudo-Loc",
                string.Empty,
				"PseudoLocResx",
                MessageImportance.Normal);

            this.BuildEngine.LogMessageEvent(args);
            foreach (var embeddedResourcesItem in this.EmbeddedResourcesItems)
            {
	            var lang = this.GetCultureInfo(embeddedResourcesItem.ItemSpec);
	            if (lang == null)
	            {
		            this.BuildEngine.LogMessageEvent(
			            new BuildMessageEventArgs(
							string.Format("Started converting Resx {0} to Pseudo-Loc", embeddedResourcesItem.ItemSpec),
							string.Empty,
							"PseudoLocResx",
							MessageImportance.Normal));


		            var outputFileName = Path.Combine(
			            Path.GetDirectoryName(embeddedResourcesItem.ItemSpec),
						Path.GetFileNameWithoutExtension(embeddedResourcesItem.ItemSpec) + string.Format(".{0}.resx", locale));

		            var strings = new Dictionary<string, object>();
		            using (var rsxr = new ResXResourceReader(embeddedResourcesItem.ItemSpec))
		            {
			            rsxr.UseResXDataNodes = true;
			            strings = rsxr.Cast<DictionaryEntry>()
									.ToDictionary(x => x.Key.ToString(), x => ((ResXDataNode)x.Value).GetValue((ITypeResolutionService)null));
		            }

		            using (ResXResourceWriter resourceWriter = new ResXResourceWriter(outputFileName))
		            {
			            foreach (var entry in strings)
			            {
				            if (entry.Value is string)
				            {
					            var value = entry.Value as string;
					            CultureInfo ci = null;

					            if (value.TryParse(out ci))
						            resourceWriter.AddResource(entry.Key, locale);
					            else
						            resourceWriter.AddResource(entry.Key, value.ToPseudo());
				            }
				            else
				            {
					            resourceWriter.AddResource(entry.Key, entry.Value);
				            }
			            }

			            this.BuildEngine.LogMessageEvent(
				            new BuildMessageEventArgs(
								string.Format("Generated pseudo-loc file {0}", outputFileName),
								string.Empty,
								"PseudoLocResx",
								MessageImportance.Normal));
			            resourceWriter.Generate();
		            }
	            }
            }

            return true;
        }

        /// <summary>
        /// The get culture info.
        /// </summary>
        /// <param name="resourceItem">
        /// The resource item.
        /// </param>
        /// <returns>
        /// The <see cref="CultureInfo"/>.
        /// </returns>
        private CultureInfo GetCultureInfo(string resourceItem)
        {           
            var fileName = Path.GetFileNameWithoutExtension(resourceItem);
	        CultureInfo ci = null;

            // assuming the file name is of the format xyz.en-us.resx, xyx.abc.en-us.resx or xyx.resx
            var lang = Path.GetExtension(fileName);
	        if (!string.IsNullOrEmpty(lang))
	        {
		        lang.Trim('.')
					.TryParse(out ci);
	        }

	        return ci;
        }
    }
}