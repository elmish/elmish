# ReactNative web request sample.

This sample uses the fetch api to make an async request out to a server.
It also demonstrates the use of `Cmd.ofPromise` commands.
The sample is hard-coded with the `https://httpbin.org/ip` endpoint which returns the IP the request is made from.
There is a restriction on iOS networking that only HTTPS requests are supported out of the box.
If you try to request an HTTP endpoint, you can find some information in [this issue](https://github.com/facebook/react-native/issues/5222#issuecomment-232932131).

## Build
0. Have the SDK of your target platform installed and working.

1. Install [react-native](https://facebook.github.io/react-native/) and test your installation
2. `npm install -g yarn`
3. Have your Android/iOS simulator or device started at this point 
4. `yarn build`
5. `react-native run-ios` *or* `react-native run-android` 
