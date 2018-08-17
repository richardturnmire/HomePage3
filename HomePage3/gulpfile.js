"use strict";

var gulp = require('gulp'),
  sass = require('gulp-sass'),
  autoprefixer = require('gulp-autoprefixer'),
  sourcemaps = require('gulp-sourcemaps'),
  cleanCSS = require('gulp-clean-css'),
  rename = require('gulp-rename'),
  uglify = require('gulp-uglify'),
  htmlmin = require('gulp-htmlmin'),
  del = require("del"),
  size = require("gulp-size"),
  soften = require("gulp-soften"),
  browserSync = require('browser-sync').create();

var folders = {
  root: "./wwwroot/"
};

var regex = {
  css: /\.css$/,
  html: /\.(html|htm)$/,
  js: /\.js$/
};

gulp.task('clean', gulp.series(() => del(['wwwroot/css/**/*'])));

gulp.task('icon1', gulp.series(function () {
  return gulp.src("node_modules/mdbootstrap/font/roboto/**.*")
    .pipe(gulp.dest('wwwroot/lib/font/roboto'));
}));

gulp.task('icon2', gulp.series(function () {
  return gulp.src("node_modules/@fortawesome/font-awesome-free/webfonts/**.*")
    .pipe(gulp.dest('wwwroot/lib/webfonts'));
}));

gulp.task('sass', gulp.series(function () {
  // sass directory
  return gulp.src('./Styles/*scss')
    .pipe(soften(4))
    //outputstyle (nested, compact, expanded, compressed)
    .pipe(sass({ outputStyle: 'compact' }).on('error', sass.logError))

    // sourcemaps
    .pipe(sourcemaps.init())
    // sourcemaps output directory
    .pipe(sourcemaps.write(('./maps')))
    // css output directory
    .pipe(gulp.dest('./wwwroot/css'));

}));

// minify css (merge + autoprefix + rename)
gulp.task('minify-css', gulp.series(function () {
  return gulp.src('./wwwroot/css/*.css')
    .pipe(cleanCSS())
    // autoprefixer
    .pipe(autoprefixer({
      browsers: ['last 15 versions'],
      cascade: false
    }))
    // minify css rename
    .pipe(rename({ suffix: '.min' }))
    // minify css output directory
    .pipe(gulp.dest('./wwwroot/css'))
    // browser sync
    .pipe(browserSync.reload({ stream: true }));
    // watch file
   
}));

// sass/css browser tracking
gulp.task('browser-sync', gulp.series(function () {
  browserSync.init({
    server: {
      baseDir: './'
    }
  });
  // watch html
}));

// Gulp task to minify JavaScript files
gulp.task('scripts', gulp.series(function () {
  return gulp.src('./Scripts/*.js')
    // Minify the file
    .pipe(uglify())
    // Output
    .pipe(gulp.dest('./wwwroot/js'));
}));

// Gulp task to minify HTML files
gulp.task('pages', gulp.series(function () {
  return gulp.src(['./*.html'])
    .pipe(htmlmin({
      collapseWhitespace: true,
      removeComments: true
    }))
    .pipe(gulp.dest('./wwwroot'));
}));

function reload(done) {
  done();
}

gulp.task('watch', gulp.parallel(() => {
  gulp.watch('./*.html').on('change', browserSync.reload);
  gulp.watch('./css/*.css', reload);
  gulp.watch('./Styles/*.scss', reload);
}));

// gulp default (sass, minify-css, browser-sync) method
gulp.task('default', gulp.parallel(gulp.series('clean', 'sass', 'minify-css'), 'scripts', 'pages'));