var path = require("path");
var webpack = require("webpack");
const CopyWebpackPlugin = require('copy-webpack-plugin');

function resolve(filePath) {
  return path.join(__dirname, filePath)
}

var babelOptions = {
  presets: [["es2015", {"modules": false}]],
  plugins: ["transform-runtime"]
}

var cfg = {
  devtool: "source-map",
  entry: resolve('./app.fsproj'),
  output: {
    publicPath: "/public",
    path: resolve('./public'),
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
  },
  plugins: [
    new CopyWebpackPlugin([
        { from: 'node_modules/todomvc-app-css/index.css', to: '.' },
        { from: 'node_modules/todomvc-common/base.css', to: '.' }
    ])
  ]
};


if (process.env.WEBPACK_DEV_SERVER) {
  cfg.entry = [
    "webpack-dev-server/client?http://localhost:8080",
    'webpack/hot/only-dev-server',
    "./out"
  ];
  cfg.plugins.push(
    new webpack.HotModuleReplacementPlugin()
  );
  cfg.module.rules.push({
    test: /\.js$/,
    exclude: /node_modules/,
    loader: "react-hot-loader"
  });
  cfg.devServer = {
    hot: true,
    contentBase: "public/",
    publicPath: "/"
  };
}

module.exports = cfg;