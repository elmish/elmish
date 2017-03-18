var path = require("path");
var webpack = require("webpack");

function resolve(filePath) {
  return path.join(__dirname, filePath)
}

var babelOptions = {
  presets: [["es2015", {"modules": false}]],
  plugins: ["transform-runtime"]
}

var cfg = {
  devtool: "source-map",
  entry: resolve('./WsClient.fsproj'),
  output: {
    path: resolve('../public'),
    publicPath: "/public",
    filename: 'bundle.js',
  },
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: "fable-loader",
          options: { babel: babelOptions }
        }
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: babelOptions
        },
      }
    ]
  },
  resolve: {
    modules: [
      "node_modules", resolve("../node_modules/")
    ]
  }
};

module.exports = cfg;