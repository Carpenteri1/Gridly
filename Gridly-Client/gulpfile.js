var gulp = require("gulp");
var exec = require("gulp-exec");
var clean = require('gulp-clean');

gulp.task('build-angular', function () {
  return gulp.src('./')
    .pipe(exec('ng build --output-path=dist'));
});
gulp.task("move-build", function () {
  return gulp.src("./dist/browser/**")
    .pipe(gulp.dest("../wwwroot/"));
});

gulp.task("clean-build", function () {
  return gulp.src(["./dist/","../wwwroot/index.csr.html"],{read: false})
    .pipe(clean({force: true}));
});

gulp.task("build", gulp.series("build-angular", "move-build", "clean-build"));
