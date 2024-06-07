# 2.3.3
* Unity SDK brought up to v7.8.3 of Native Android SDK [#58](https://github.com/SourcePointUSA/unity-sdk/pull/58) which fixes the following:
* [DIA-4087](https://sourcepoint.atlassian.net/browse/DIA-4087) Fixed: Empty consent when GDPR/CCPA applies is false
* Also, in Unity SDK version 2.3.3, due to Java version upgrade as well as Gradle upgrade to version 7.5 in native Android SDK 7.8.2 (which is a dependency of Unity SDK), CMP SDK is now supported on Unity version 2022.3 and higher. However, we are able to build with Gradle v7.2 which is built-in in Unity 2022.3, so using Gradle 7.5 is not required.
* Please also note that in order to build a project you need to use a custom template for Gradle, please refer to "Plugins\Android\mainTemplate.gradle", lines 41-43.
* Various minor improvements.

# 2.3.2
* Unity SDK brought v7.6.8 iOS [#55](https://github.com/SourcePointUSA/unity-sdk/pull/55)
* [DIA-4048] (https://sourcepoint.atlassian.net/browse/DIA-4048) Fix: of the null authId interpreted as "" (an empty string) [#55](https://github.com/SourcePointUSA/unity-sdk/pull/55) 
* Fix: JsonUnwrapper on iOS has incorrect key for VendorGrant [#57](https://github.com/SourcePointUSA/unity-sdk/pull/57) by @OmarVector 

# 2.3.1
* [DIA-3844](https://sourcepoint.atlassian.net/browse/DIA-3844) Destroy CMPiOSListenerHelper GameObject in Dispose() call and re-initialize it in CMP.Initialize() call if needed. Also, ClearAllData() method for IOS clears cached value in c# [#53](https://github.com/SourcePointUSA/unity-sdk/pull/53)

# 2.3.0
* [DIA-3046](https://sourcepoint.atlassian.net/browse/DIA-3046) Implemented USNat company for IOS [#47](https://github.com/SourcePointUSA/unity-sdk/pull/47)
* [DIA-3630](https://sourcepoint.atlassian.net/browse/DIA-3630) Implemented USNat company for Android [#48](https://github.com/SourcePointUSA/unity-sdk/pull/48)
* [DIA-3666](https://sourcepoint.atlassian.net/browse/DIA-3666) Updated example scene with adaptive UI [#49](https://github.com/SourcePointUSA/unity-sdk/pull/49)

# 2.2.2
* [DIA-3628](https://sourcepoint.atlassian.net/browse/DIA-3628), [DIA-3629](https://sourcepoint.atlassian.net/browse/DIA-3629) Implemented GCM company for IOS and Android [#41](https://github.com/SourcePointUSA/unity-sdk/pull/41), [#43](https://github.com/SourcePointUSA/unity-sdk/pull/43)
* [DIA-3350](https://sourcepoint.atlassian.net/browse/DIA-3350) Unity SDK brought to v7.7.1 Android & v.7.6.3 iOS [#44](https://github.com/SourcePointUSA/unity-sdk/pull/44), [#42](https://github.com/SourcePointUSA/unity-sdk/pull/42)
* [DIA-2585](https://sourcepoint.atlassian.net/browse/DIA-2585) Implemented cutomConsentTo (for iOS and Android) / deleteCustomConsent (for Android) features [#42](https://github.com/SourcePointUSA/unity-sdk/pull/42)
* Miscellaneous fixes

# 2.2.1
* [DIA-3594](https://sourcepoint.atlassian.net/browse/DIA-3594) Unity SDK brought to v7.7.0 Android & v.7.6.0 iOS [#36](https://github.com/SourcePointUSA/unity-sdk/pull/36), [#37](https://github.com/SourcePointUSA/unity-sdk/pull/37)

# 2.2.0
* [DIA-3263](https://sourcepoint.atlassian.net/browse/DIA-3263) Update of the Native Android SDK to 7.6.0 (fix of no internet connection issue on Android) [#30](https://github.com/SourcePointUSA/unity-sdk/pull/30)
* [DIA-2815](https://sourcepoint.atlassian.net/browse/DIA-2815) Implemented new swift bridge for IOS [#21](https://github.com/SourcePointUSA/unity-sdk/pull/21)
* [DIA-3045](https://sourcepoint.atlassian.net/browse/DIA-3045) Remove TextMeshPro asset [#22](https://github.com/SourcePointUSA/unity-sdk/pull/22)
* [DIA-2586](https://sourcepoint.atlassian.net/browse/DIA-2586) Implemented ClearAllData() feature for Android, iOS [#24](https://github.com/SourcePointUSA/unity-sdk/pull/24)
* [DIA-2584](https://sourcepoint.atlassian.net/browse/DIA-2584) Implemented cutomConsentTo (for iOS and Android) / deleteCustomConsent (for iOS) features [#26](https://github.com/SourcePointUSA/unity-sdk/pull/26), [#34](https://github.com/SourcePointUSA/unity-sdk/pull/34)
* Improve Android logging experience: introduce CmpAndroidLoggerProxy C#-native implementation of logger derived from native com.sourcepoint.cmplibrary.exception.Logger interface [#33](https://github.com/SourcePointUSA/unity-sdk/pull/33)
* Get rid of System.Threading.Tasks.Extensions.dll, Microsoft.Bcl.AsyncInterfaces.dll, System.Buffers.dll, System.Memory.dll, System.Numerics.Vectors.dll, System.Runtime.CompilerServices.Unsafe.dll, System.Text.Encodings.Web.dll which were used by System.Text.Json.dll
* Newtonsoft JSON parsing: safety measures and improvements!
* CCPA campaign added to the PrivacySettings.cs script for the Example App

# 2.1.7
* [DIA-3263](https://sourcepoint.atlassian.net/browse/DIA-3263) DIA-3263 Fix internet connection issue [#32](https://github.com/SourcePointUSA/unity-sdk/pull/32)
* Added native support for v7.6.0 Android

# 2.1.6
* [DIA-3423](https://sourcepoint.atlassian.net/browse/DIA-3423)[DIA-3349](https://sourcepoint.atlassian.net/browse/DIA-3349) DIA-3423, DIA-3349 Fix Unity racing conditions [#28](https://github.com/SourcePointUSA/unity-sdk/pull/28)

# 2.1.5
* [DIA-2811](https://sourcepoint.atlassian.net/browse/DIA-2811) DIA-2811 Removed code signing for IOS [#25](https://github.com/SourcePointUSA/unity-sdk/pull/25)

# 2.1.4
* [DIA-2808](https://sourcepoint.atlassian.net/browse/DIA-2808) DIA-2808 Implemented JSON parsing with Newtonsoft for Android [#20](https://github.com/SourcePointUSA/unity-sdk/pull/20)

# 2.1.3
* Fixed `onConsentReady` - `onSpFinished` race condition for android

# 2.1.2
* Miscellaneous fixes
* Improve logging system and error handling
* Enabling improved logging in onSpFinished

# 2.1.1
* [DIA-2884](https://sourcepoint.atlassian.net/browse/DIA-2884) Fixed enum error for Android 8.0 [#17](https://github.com/SourcePointUSA/unity-sdk/pull/17)
* [DIA-2583](https://sourcepoint.atlassian.net/browse/DIA-2583) Import scene with `TextMesh Pro` [#16](https://github.com/SourcePointUSA/unity-sdk/pull/16)

# 2.1.0
* [DIA-2691](https://sourcepoint.atlassian.net/browse/DIA-2691) Added `customActionId` support [#14](https://github.com/SourcePointUSA/unity-sdk/pull/14)
* [DIA-2686](https://sourcepoint.atlassian.net/browse/DIA-2686) Update `projPath` definition in `OnPostProcessBuild` script [#13](https://github.com/SourcePointUSA/unity-sdk/pull/13)
* [DIA-2582](https://sourcepoint.atlassian.net/browse/DIA-2682) Update `README` with `GetSpConsent()` description [#12](https://github.com/SourcePointUSA/unity-sdk/pull/12)

# 2.0.0 (Aug, 18, 2023)
* Added native support for v7 for v7.2.7 Android & v.7.3.0 iOS 
* [DIA-2551](https://sourcepoint.atlassian.net/browse/DIA-2551), [DIA-2552](https://sourcepoint.atlassian.net/browse/DIA-2552) Unity SDK brought to v7.2.7 Android & v.7.3.0 iOS [#9](https://github.com/SourcePointUSA/unity-sdk/pull/9)
* [DIA-2473](https://sourcepoint.atlassian.net/browse/DIA-2473) Optimisation of code for UPM [#4](https://github.com/SourcePointUSA/unity-sdk/pull/4), [#5](https://github.com/SourcePointUSA/unity-sdk/pull/5)
* [DIA-2026](https://sourcepoint.atlassian.net/browse/DIA-2026) Update / Improve Unity SDK [#3](https://github.com/SourcePointUSA/unity-sdk/pull/3)
* Set consent ui prensentation style to over fullscreen
* Update README

# 1.0.0 (Jul, 26, 2023)
Added native support for native v6 both Android and iOS 
* [DIA-2026](https://sourcepoint.atlassian.net/browse/DIA-2026) Update / Improve Unity SDK [#3](https://github.com/SourcePointUSA/unity-sdk/pull/3)
