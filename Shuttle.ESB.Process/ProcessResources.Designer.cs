﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shuttle.ESB.Process {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ProcessResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ProcessResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shuttle.ESB.Process.ProcessResources", typeof(ProcessResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Process type &apos;{0}&apos; cannot process message type &apos;{1}&apos; since the correlation id of &apos;{2}&apos; is not a valid guid..
        /// </summary>
        public static string InvalidCorrelationGuid {
            get {
                return ResourceManager.GetString("InvalidCorrelationGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Process assessor type with AssemblyQualifiedName &apos;{0}&apos; does not have a default constructor..
        /// </summary>
        public static string MissingProcessAssessorConstructor {
            get {
                return ResourceManager.GetString("MissingProcessAssessorConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Process type with AssemblyQualifiedName &apos;{0}&apos; could not be instantiated..
        /// </summary>
        public static string ProcessFactoryFunctionException {
            get {
                return ResourceManager.GetString("ProcessFactoryFunctionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message type &apos;{0}&apos; maps to more than one process type.  You will need to register a resolver to determine which type of process to instantiate..
        /// </summary>
        public static string ResolverRequiredForMultipleMappingsException {
            get {
                return ResourceManager.GetString("ResolverRequiredForMultipleMappingsException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message type &apos;{0}&apos; does not map to any process type.  You will need to register a resolver to determine which type of process to instantiate or add a mapping..
        /// </summary>
        public static string ResolverRequiredForNoMappingException {
            get {
                return ResourceManager.GetString("ResolverRequiredForNoMappingException", resourceCulture);
            }
        }
    }
}
