var gulp = require("gulp");
var exec = require("gulp-exec");
var clean = require("gulp-clean");

// ---- Angular ----
gulp.task("ng-build", function () {
  return gulp
    .src(".", { allowEmpty: true })
    .pipe(exec("ng build --output-path=dist"))
    .pipe(exec.reporter());
});

gulp.task("ng-move-build", function () {
  return gulp
    .src("./dist/browser/**/*", { allowEmpty: true })
    .pipe(gulp.dest("../wwwroot/"));
});

gulp.task("ng-clean-build", function () {
  return gulp
    .src(["./dist/", "../wwwroot/index.csr.html"], { allowEmpty: true })
    .pipe(clean({ force: true }));
});

// ---- dotnet ----
gulp.task("dotnet-restore", function () {
  return gulp
    .src(".", { allowEmpty: true })
    .pipe(exec(`dotnet restore`))
    .pipe(exec.reporter());
});

gulp.task("dotnet-build", function () {
  return gulp
    .src(".", { allowEmpty: true })
    .pipe(exec(`dotnet build`))
    .pipe(exec.reporter());
});

gulp.task("dotnet-run", function () {
  return gulp
    .src(".", { allowEmpty: true })
    .pipe(exec(`dotnet run --no-build --project`))
    .pipe(exec.reporter());
});

gulp.task(
  "build-angular",
  gulp.series(
    "ng-build",
    "ng-move-build",
    "ng-clean-build",
    "dotnet-restore",
    "dotnet-build",
    "dotnet-run"
  )
);
