var gulp = require("gulp");
var exec = require("gulp-exec");
var clean = require('gulp-clean');

gulp.task('ng-build', function () {
  return gulp.src('./')
    .pipe(exec('ng build --output-path=dist'));
});

gulp.task("ng-move-build", function () {
  return gulp.src(["./dist/browser/*","./dist/browser/**"],
    {encoding: false},
    {overwrite: true})
    .pipe(gulp.dest("../wwwroot/"),gulp.dest("../wwwroot/media/"));
});

gulp.task("ng-clean-build", function () {
  return gulp.src(["./dist/","../wwwroot/index.csr.html"],{allowEmpty: true})
    .pipe(clean({force: true},));
});

gulp.task("build-angular", gulp.series(
  "ng-build",
  "ng-move-build",
  "ng-clean-build"
));
