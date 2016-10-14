import { Util } from "fable-core";
import * as react from "react";
import { View } from "react-native";
import { TouchableHighlight } from "react-native";
import { Text as _Text } from "react-native";
import { ProgramModule } from "./fable_external/elmish-735012563";
import { Component } from "react";
export class Model {
  constructor(count) {
    this.count = count;
  }

  Equals(other) {
    return Util.equalsRecords(this, other);
  }

  CompareTo(other) {
    return Util.compareRecords(this, other);
  }

}
Util.setInterfaces(Model.prototype, ["FSharpRecord", "System.IEquatable", "System.IComparable"], "AwesomeApp.Model");
export class Msg {
  constructor(caseName, fields) {
    this.Case = caseName;
    this.Fields = fields;
  }

  Equals(other) {
    return Util.equalsUnions(this, other);
  }

  CompareTo(other) {
    return Util.compareUnions(this, other);
  }

}
Util.setInterfaces(Msg.prototype, ["FSharpUnion", "System.IEquatable", "System.IComparable"], "AwesomeApp.Msg");
export function init() {
  return new Model(0);
}
export function update(msg, _arg1) {
  return msg.Case === "Decrement" ? new Model(_arg1.count - 1) : new Model(_arg1.count + 1);
}
export function view(_arg1, dispatch) {
  const onClick = msg => () => {
    dispatch(msg);
  };

  return react.createElement(View, {}, ...[react.createElement(TouchableHighlight, {
    style: {
      backgroundColor: "#428bca",
      borderRadius: 4,
      margin: 5
    },
    underlayColor: "#5499C4",
    onPress: onClick(new Msg("Decrement", []))
  }, ...[react.createElement(_Text, {
    style: {
      color: "#FFFFFF",
      textAlign: "center",
      margin: 5,
      fontSize: 15
    }
  }, ..."-")]), react.createElement(_Text, {}, ...String(_arg1.count)), react.createElement(TouchableHighlight, {
    style: {
      backgroundColor: "#428bca",
      borderRadius: 4,
      margin: 5
    },
    underlayColor: "#5499C4",
    onPress: onClick(new Msg("Increment", []))
  }, ...[react.createElement(_Text, {
    style: {
      color: "#FFFFFF",
      textAlign: "center",
      margin: 5,
      fontSize: 15
    }
  }, ..."+")])]);
}
export const program = ProgramModule.withConsoleTrace(ProgramModule.mkSimple(() => init(), msg => arg10_ => update(msg, arg10_)));
export class App extends Component {
  constructor() {
    super();
    this.this = {
      contents: null
    };
    const _this = this.this;
    this.this.contents = this;
    this.dispatch = ProgramModule.run(state => {
      this.safeState(state);
    }, program);
    this["init@56"] = 1;
  }

  componentDidMount() {
    this.props = true;
  }

  render() {
    return view(this.state, this.dispatch);
  }

  safeState(state) {
    const matchValue = this.this.contents.props;

    if (matchValue) {
      this.this.contents.setState(state);
    } else {
      this.this.contents.state = state;
    }
  }

}
Util.setInterfaces(App.prototype, [], "AwesomeApp.App");