"use strict";
// folders
var folders = {
  root: "./wwwroot/"
};

var regex = {
  css: /\.css$/,
  html: /\.(html|htm)$/,
  js: /\.js$/
};

var config = {
  threshold: '1kb'
};

var gulp = require("gulp"),
  htmlclean = require("htmlclean"),
  fs = require("fs"),
  concat = require("gulp-concat"),
  less = require("gulp-less"),
  sass = require("gulp-sass"),
  cleanCSS = require('gulp-clean-css'),
  cssmin = require("gulp-cssmin"),
  htmlmin = require("gulp-htmlmin"),
  uglify = require("gulp-uglify"),
  gzip = require("gulp-gzip"),
  merge = require("merge-stream"),
  del = require("del"),
  sourcemaps = require('gulp-sourcemaps'),
  bundleconfig = require("./bundleconfig.json");




gulp.task("less", gulp.series(function () {
  return gulp.src('Styles/SiteLess.less')
    .pipe(less())
    .pipe(concat('SiteLess.min.css'))
    .pipe(cssmin())
    .pipe(gulp.dest('wwwroot/css'));
}));

 

gulp.task('sass1', gulp.series(function () {
  return gulp.src(["Styles/SiteScss.scss", "Styles/custom_Bootstrap.scss", "Styles/custom_MDBootstrap.scss"])
    .pipe(sass().on('error', sass.logError))
    .pipe(gulp.dest('./temp'));
}));

 
gulp.task('icon2', gulp.series(function () {
  return gulp.src("node_modules/@fortawesome/font-awesome-free/webfonts/**.*")
    .pipe(gulp.dest('wwwroot/lib/webfonts'));
}));


gulp.task('minify-css', gulp.series(() => {
  return gulp.src('./temp/*.css')
    .pipe(sourcemaps.init())
    .pipe(cleanCSS())
    
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('wwwroot/css'));
}));

gulp.task('gzip1', gulp.series(function () {

  return gulp.src(['wwwroot/**/*.min.js', 'wwwroot/**/*.min.css'])
    .pipe(gzip(config))
    .pipe(gulp.dest('wwwroot'));
}));
gulp.task('default', gulp.series('icon1', 'icon2', 'sass1', 'minify-css'));

gulp.task("min:js", gulp.series(function () {
  var tasks = getBundles(regex.js).map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(uglify())
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
}));

gulp.task("min:css", gulp.series(function () {
  var tasks = getBundles(regex.css).map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(cssmin())
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
}));

gulp.task("min:html", gulp.series(function () {
  var tasks = getBundles(regex.html).map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
}));

gulp.task("clean", gulp.series(function () {
  var files = bundleconfig.map(function (bundle) {
    return bundle.outputFileName;
  });

  return del(files);
}));

gulp.task("min", gulp.series("min:js", "min:css", "min:html"));

gulp.task("watch", gulp.series(function () {
  getBundles(regex.js).forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:js"]);
  });

  getBundles(regex.css).forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:css"]);
  });

  getBundles(regex.html).forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:html"]);
  });
}));

function getBundles(regexPattern) {
  return bundleconfig.filter(function (bundle) {
    return regexPattern.test(bundle.outputFileName);
  });
}