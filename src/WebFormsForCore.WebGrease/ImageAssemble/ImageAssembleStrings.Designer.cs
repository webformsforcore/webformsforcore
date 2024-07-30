﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebGrease.ImageAssemble {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ImageAssembleStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ImageAssembleStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WebGrease.ImageAssemble.ImageAssembleStrings", typeof(ImageAssembleStrings).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Additional Details:
        ///
        ///Image Name: {0}
        ///Sprite Name: {1}.
        /// </summary>
        internal static string AdditionalDetailsMessage {
            get {
                return ResourceManager.GetString("AdditionalDetailsMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No action performed. There was an error parsing BitDepth for input images:
        ///{0}.
        /// </summary>
        internal static string BitDepthParsingErrorMessage {
            get {
                return ResourceManager.GetString("BitDepthParsingErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Directory does not exist: {0}. 
        ///Parameter Name - {1}.
        /// </summary>
        internal static string DirectoryDoesNotExistMessage {
            get {
                return ResourceManager.GetString("DirectoryDoesNotExistMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No action performed. Duplicate image filepaths are present for image - &apos;{1}&apos; in parameter - {0}.
        /// </summary>
        internal static string DuplicateInputFilePathsMessage {
            get {
                return ResourceManager.GetString("DuplicateInputFilePathsMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 8-bit PNGs are not supported for spriting..
        /// </summary>
        internal static string EightBitPNGCannotbeSpritedMessage {
            get {
                return ResourceManager.GetString("EightBitPNGCannotbeSpritedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No action performed. Following image files did not exist:.
        /// </summary>
        internal static string IgnoredFilesMessage {
            get {
                return ResourceManager.GetString("IgnoredFilesMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation failed while replacing assembled image name: &apos;{0}&apos; with hashed name..
        /// </summary>
        internal static string ImageHashNameUpdateFailedMessage {
            get {
                return ResourceManager.GetString("ImageHashNameUpdateFailedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file does not have a valid image format.
        ///
        ///Or
        ///
        ///GDI+ does not support the pixel format of the file..
        /// </summary>
        internal static string ImageLoadOutofMemoryExceptionMessage {
            get {
                return ResourceManager.GetString("ImageLoadOutofMemoryExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} and {1}.
        /// </summary>
        internal static string ImagePositionValues {
            get {
                return ResourceManager.GetString("ImagePositionValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The image was saved with the wrong image format.
        ///
        ///Or
        ///
        ///The image was saved to the same file it was created from.
        ///
        ///Or
        ///
        ///Access is denied to the path - {0}..
        /// </summary>
        internal static string ImageSaveExternalExceptionMessage {
            get {
                return ResourceManager.GetString("ImageSaveExternalExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} and {1} should not be used together. Only one parameter should be used at one time..
        /// </summary>
        internal static string InputFilesDuplicateParameterMessage {
            get {
                return ResourceManager.GetString("InputFilesDuplicateParameterMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image position is missing for the input image file {0}. Please provide image position (L for Left / R for Right) after image path separated by Pipe symbol (|). E.g. /f:C:\1.gif|L;C:\2.gif|R.
        /// </summary>
        internal static string InputFilesMissingPositionMessage {
            get {
                return ResourceManager.GetString("InputFilesMissingPositionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input files must contain both Path and Position for each image. Input file value {0} is missing Path and/or Position value. Please provide image position (L for Left / R for Right) after image path separated by Pipe symbol (|). E.g. /f:C:\1.gif|L;C:\2.gif|R.
        /// </summary>
        internal static string InputFilesPathAndPositionMessage {
            get {
                return ResourceManager.GetString("InputFilesPathAndPositionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no InputImages found in InputImageList. Please verify InputImageList is specified correctly. Alternatively, provide either /inputdirectory or /inputfilepaths parameter to specify input images..
        /// </summary>
        internal static string InputImageListNoImageMessage {
            get {
                return ResourceManager.GetString("InputImageListNoImageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value {0} is specified for Image {1}. Please provide valid value for Image Position. Valid image position values are {2}..
        /// </summary>
        internal static string InvalidImagePositionMessage {
            get {
                return ResourceManager.GetString("InvalidImagePositionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input parameter &apos;{0}&apos; is not supported..
        /// </summary>
        internal static string InvalidInputParameterMessage {
            get {
                return ResourceManager.GetString("InvalidInputParameterMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value for input parameter - {0}.
        /// </summary>
        internal static string InvalidInputParameterValueMessage {
            get {
                return ResourceManager.GetString("InvalidInputParameterValueMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value {0} specified for padding between images assembled together in a sprite. Valid padding values are between {1} and {2} pixels (inclusive)..
        /// </summary>
        internal static string InvalidPaddingValueMessage {
            get {
                return ResourceManager.GetString("InvalidPaddingValueMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mandatory Input Parameter(s) are missing. Please provide mandatory parameter(s) - 
        ///{0}.
        /// </summary>
        internal static string MissingInputParameterMessage {
            get {
                return ResourceManager.GetString("MissingInputParameterMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no files present to process under folder: {0}.
        /// </summary>
        internal static string NoInputFilesMessage {
            get {
                return ResourceManager.GetString("NoInputFilesMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no image files provided to process..
        /// </summary>
        internal static string NoInputFileToProcessMessage {
            get {
                return ResourceManager.GetString("NoInputFileToProcessMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no input parameters provided. Please provide all mandatory parameters.
        ///
        ///{0}.
        /// </summary>
        internal static string NoInputParametersMessage {
            get {
                return ResourceManager.GetString("NoInputParametersMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Spriting of {0} bpp PNGs is not supported by this tool..
        /// </summary>
        internal static string PNGBitDepthNotSupportedMessage {
            get {
                return ResourceManager.GetString("PNGBitDepthNotSupportedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Single input image cannot be sprited..
        /// </summary>
        internal static string SingleInputImageCannotBeSpritedMessage {
            get {
                return ResourceManager.GetString("SingleInputImageCannotBeSpritedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Single PNG image having {0} bpp cannot be sprited..
        /// </summary>
        internal static string SinglePNGCannotBeSpritedMessage {
            get {
                return ResourceManager.GetString("SinglePNGCannotBeSpritedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while performing the requested operation:
        ///{0}.
        /// </summary>
        internal static string ToolCommandLineErrorMessage {
            get {
                return ResourceManager.GetString("ToolCommandLineErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Successfully completed the action. Log file located at: {0}.
        /// </summary>
        internal static string ToolSuccessfulCompletionMessage {
            get {
                return ResourceManager.GetString("ToolSuccessfulCompletionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tool Usage:
        ///
        ///Provide parameters from following list:
        ///
        ////inputdirectory  - [Mandatory] Input directory path that contains all the images to be assembled. This parameter is not recursive. Shorthand - /i. E.g. /inputdirectory:C:\Images.
        ///&apos;/inputdirectory&apos; and &apos;/inputfilepaths&apos; should not be used together. Only one parameter should be used at one time.
        ///
        ////inputfilepaths  - [Mandatory] Semicolon (;) separated input file paths for images that need to be assembled. Providing image position for each image is mandatory [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ToolUsageMessage {
            get {
                return ResourceManager.GetString("ToolUsageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value is not specified for {0}.
        /// </summary>
        internal static string ValueMissingForInputParameterMessage {
            get {
                return ResourceManager.GetString("ValueMissingForInputParameterMessage", resourceCulture);
            }
        }
    }
}
