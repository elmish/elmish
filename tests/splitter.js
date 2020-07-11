module.exports = {
    entry: resolve("./Fable.Elmish.Tests.fsproj"),
    outDir: resolve("./bin"),
    babel: {
        plugins: ["transform-es2015-modules-commonjs"],
    }
};

function resolve(path) {
    return require("path").join(__dirname, path);
}