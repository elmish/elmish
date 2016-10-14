import * as coreJs from "core-js/shim";
import {AppRegistry} from 'react-native';
import {App} from './out/App';

AppRegistry.registerComponent('counter', () => App);
