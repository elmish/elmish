import { List } from "fable-core";
import { Async } from "fable-core";
import { AsyncBuilder } from "fable-core";
import { Util } from "fable-core";
import { MailboxProcessor } from "fable-core";
import { Seq } from "fable-core";
export const CmdModule = function ($exports) {
  const none = $exports.none = function none() {
    return new List();
  };

  const ofMsg = $exports.ofMsg = function ofMsg(msg) {
    return List.ofArray([dispatch => {
      dispatch(msg);
    }]);
  };

  const map = $exports.map = function map(f, cmd) {
    return List.map(g => $var2 => g((post => $var1 => post(f($var1)))($var2)), cmd);
  };

  const batch = $exports.batch = function batch(cmds) {
    return List.collect(x => x, cmds);
  };

  const ofAsync = $exports.ofAsync = function ofAsync(task, arg, ofSuccess, ofError) {
    const bind = dispatch => (builder_ => builder_.Delay(() => builder_.Bind(Async.catch(task(arg)), _arg1 => {
      dispatch(_arg1.Case === "Choice2Of2" ? ofError(_arg1.Fields[0]) : ofSuccess(_arg1.Fields[0]));
      return builder_.Zero();
    })))(AsyncBuilder.singleton);

    return List.ofArray([$var3 => (arg00 => {
      Async.startImmediate(arg00);
    })(bind($var3))]);
  };

  const ofFunc = $exports.ofFunc = function ofFunc(task, arg, ofSuccess, ofError) {
    const bind = dispatch => {
      try {
        ($var4 => dispatch(ofSuccess($var4)))(task(arg));
      } catch (x) {
        ($var5 => dispatch(ofError($var5)))(x);
      }
    };

    return List.ofArray([bind]);
  };

  const ofSub = $exports.ofSub = function ofSub(sub) {
    return List.ofArray([sub]);
  };

  return $exports;
}({});
export class Program {
  constructor(init, update, subscribe) {
    this.init = init;
    this.update = update;
    this.subscribe = subscribe;
  }

}
Util.setInterfaces(Program.prototype, ["FSharpRecord"], "Elmish.Program");
export const ProgramModule = function ($exports) {
  const mkProgram = $exports.mkProgram = function mkProgram(init, update) {
    return new Program(init, update, _arg1 => CmdModule.none());
  };

  const mkSimple = $exports.mkSimple = function mkSimple(init, update) {
    return new Program($var6 => (state => [state, CmdModule.none()])(init($var6)), msg => $var7 => (state => [state, CmdModule.none()])(update(msg)($var7)), _arg1 => CmdModule.none());
  };

  const withSubscription = $exports.withSubscription = function withSubscription(subscribe, program) {
    return new Program(program.init, program.update, subscribe);
  };

  const withConsoleTrace = $exports.withConsoleTrace = function withConsoleTrace(program) {
    const trace = text => msg => model => {
      console.log(text, model, msg);
      return program.update(msg)(model);
    };

    const update = trace("Updating:");
    return new Program(program.init, update, program.subscribe);
  };

  const withTrace = $exports.withTrace = function withTrace(program, trace) {
    const update = msg => model => {
      trace(msg)(model);
      return program.update(msg)(model);
    };

    return new Program(program.init, update, program.subscribe);
  };

  const runWith = $exports.runWith = function runWith(arg, setState, program) {
    const patternInput = program.init(arg);
    const inbox = MailboxProcessor.start(mb => {
      const loop = state => (builder_ => builder_.Delay(() => builder_.Combine(builder_.TryWith(builder_.Delay(() => {
        setState(state);
        return builder_.Zero();
      }), _arg1 => {
        console.error("unable to setState:", state, _arg1);
        return builder_.Zero();
      }), builder_.Delay(() => builder_.Bind(mb.receive(), _arg2 => builder_.TryWith(builder_.Delay(() => {
        const patternInput_1 = program.update(_arg2)(state);
        Seq.iterate(sub => {
          sub(arg00 => {
            mb.post(arg00);
          });
        }, patternInput_1[1]);
        return builder_.ReturnFrom(loop(patternInput_1[0]));
      }), _arg3 => {
        console.error("unable to update:", _arg3);
        return builder_.ReturnFrom(loop(state));
      }))))))(AsyncBuilder.singleton);

      return loop(patternInput[0]);
    });
    Seq.iterate(sub => {
      sub(arg00 => {
        inbox.post(arg00);
      });
    }, List.append(program.subscribe(patternInput[0]), patternInput[1]));
    return arg00 => {
      inbox.post(arg00);
    };
  };

  const run = $exports.run = function run(setState, program) {
    return runWith(null, setState, program);
  };

  return $exports;
}({});