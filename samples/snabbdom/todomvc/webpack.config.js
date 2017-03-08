var path = require("path");
var webpack = require("webpack");

var cfg = {
  devtool: "source-map",
  entry: "./out/todomvc.js",
  output: {
    path: path.join(__dirname, "public"),
    publicPath: "/public",
    filename: "bundle.js"
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        loader: "source-map-loader",
        enforce: "pre"
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        loader: 'babel-loader',
        options: {
          plugins: ["transform-runtime"]
        },
      }
    ]
  },
  resolve: {
    modules: [
      "node_modules", path.resolve("../node_modules/")
    ]
  }
};

if (process.env.WEBPACK_DEV_SERVER) {
  cfg.entry = [
    "webpack-dev-server/client?http://localhost:8080",
    'webpack/hot/only-dev-server',
    "./out"
  ];
  cfg.plugins = [
    new webpack.HotModuleReplacementPlugin()
  ];
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