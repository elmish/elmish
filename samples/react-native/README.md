Native samples
=======
Note: Running Native samples requires react-native and platform SDKs installation.
Certain samples do not have the native infrastructure checked in. Do not be alarmed, the `ios/` and `android/` subfolders can be provisioned by the react-native CLI or directly copied across the projects.

## Using the cli example
> react-native init awesome
> cp -r awesome/ios visibility

Make sure the package.json `id` matches the react-native's project name. This should match the `runnable` name you register in the `_.index.js` files.