var path = require("path");
var fs = require("fs-extra");
var fable = require("fable-compiler");

var targets = {
    all() {
        return fable.promisify(fs.remove, "npm")
            .then(_ => fable.compile())
            .then(_ => fable.compile({target: "umd"}))
            .then(_ => fable.promisify(fs.copy, "package.json", "npm/package.json"))
            .then(_ => fable.promisify(fs.copy, "README.md", "npm/README.md"))
            .then(_ => fable.promisify(fs.readFile, "RELEASE_NOTES.md"))
            .then(line => {
                var version = /\d[^\s]*/.exec(line)[0];
                return fable.runCommand("npm", "npm version " + version);
            });
    }
}

targets[process.argv[2] || "all"]().catch(err => {
    console.log("[ERROR] " + err);
    process.exit(-1);
});
