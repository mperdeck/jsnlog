

   ----------------------------------------------------------------------
              README file for Bundle Transformer: LESS 1.8.33

   ----------------------------------------------------------------------

          Copyright 2014 Andrey Taritsyn - http://www.taritsyn.ru
		  

   ===========
   DESCRIPTION
   ===========
   BundleTransformer.Less contains translator-adapter `LessTranslator` 
   (supports LESS (http://lesscss.org) version 1.7.0). This adapter makes
   translation of LESS-code to CSS-code. Also contains debugging 
   HTTP-handler `LessAssetHandler`, which is responsible for text output 
   of translated LESS-asset.
   
   BundleTransformer.Less does not support the string interpolation in
   file paths. 
   
   As a JS-engine is used the JavaScript Engine Switcher library 
   (http://github.com/Taritsyn/JavaScriptEngineSwitcher). For correct 
   working of this module is recommended to install one of the following 
   NuGet packages: JavaScriptEngineSwitcher.Msie or 
   JavaScriptEngineSwitcher.V8.
   
   =============
   RELEASE NOTES
   =============
   LESS was updated to commit fab1401fc0.
   
   ====================
   POST-INSTALL ACTIONS
   ====================
   For correct working of this module is recommended to install one of 
   the following NuGet packages: JavaScriptEngineSwitcher.Msie or 
   JavaScriptEngineSwitcher.V8. After package is installed, need set a 
   name of JavaScript engine (for example, `MsieJsEngine`) to the `name`
   attribute of `/configuration/bundleTransformer/less/jsEngine` 
   configuration element.

   To use a debugging HTTP-handler in the IIS Classic mode, you need add
   to the `/configuration/system.web/httpHandlers` element of the 
   Web.config file a following code:

   <add
	path="*.less" verb="GET"
	type="BundleTransformer.Less.HttpHandlers.LessAssetHandler, BundleTransformer.Less" />
   
   =============
   DOCUMENTATION
   =============
   See documentation on CodePlex - 
   http://bundletransformer.codeplex.com/documentation