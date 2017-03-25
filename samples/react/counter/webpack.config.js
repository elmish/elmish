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
        exclude: /node_modules[\\\/](?!fable-)/,
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

if (process.env.WEBPACK_DEV_SERVER) {
  cfg.entry = [
    "webpack-dev-server/client?http://localhost:8080",
    'webpack/hot/only-dev-server',
    resolve("./app.fsproj")
  ];
  cfg.plugins = [
    new webpack.HotModuleReplacementPlugin()
  ];
  cfg.module.loaders = [{
    test: /\.js$/,
    exclude: /node_modules/,
    loader: "react-hot-loader"
  }];
  cfg.devServer = {
    hot: true,
    contentBase: "public/",
    publicPath: "/"
  };
}

module.exports = cfg;