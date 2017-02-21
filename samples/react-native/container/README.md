# ReactNative container sample.

This sample shows 
* how you can reuse components while keeping their local state separated.
* how to use RemoteDev time-traveling debugger.

## Build
0. Have the SDK of your target platform installed and working.

1. Install [react-native](https://facebook.github.io/react-native/) and test your installation
2. `npm install -g yarn`
3. Have your Android/iOS simulator or device started at this point 
4. `yarn build`
    - Optionally, inject local RemoteDev server into RN packager startup: `yarn i-remotedev`
5. `react-native run-ios` *or* `react-native run-android` 
