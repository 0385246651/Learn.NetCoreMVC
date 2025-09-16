const gulp = require("gulp");
const sass = require("gulp-sass")(require("sass"));
const postcss = require("gulp-postcss");
const autoprefixer = require("autoprefixer");
const concat = require("gulp-concat");

const paths = {
  scss: "./assets/scss/**/*.scss",
  css: "./wwwroot/css/",
};

// Compile SCSS -> CSS (có sourcemap để dev debug)
function buildCss() {
  return (
    gulp
      .src(paths.scss)
      //, { sourcemaps: false })// tắt sourcemap khi build
      .pipe(sass().on("error", sass.logError))
      .pipe(postcss([autoprefixer()]))
      .pipe(gulp.dest(paths.css, { sourcemaps: "." }))
  );
}

// Minify CSS (KHÔNG sourcemap)
// function minifyCss() {
//   return gulp
//     .src(paths.css + "*.css") // lấy CSS đã build
//     .pipe(cleanCSS({ sourceMap: false })) // tắt sourceMap
//     .pipe(gulp.dest(paths.minCss));
// }

// Concat CSS (optional, nếu muốn gộp tất cả css lại)
function concatCss() {
  return gulp
    .src(paths.css + "*.css")
    .pipe(concat("site.css")) // không còn .min.css nữa
    .pipe(gulp.dest(paths.css));
}

// Watch task
function watchFiles() {
  gulp.watch(
    paths.scss,
    gulp.series(
      buildCss,
      // minifyCss
      concatCss
    )
  );
}

// Default task
exports.default = gulp.series(
  buildCss,
  concatCss,
  // minifyCss
  watchFiles
);
