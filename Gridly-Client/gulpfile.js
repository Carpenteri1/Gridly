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

gulp.task("ng-serve", function () {
  console.log("🚀 Starting client server...");
  gulp
      .src(".", { allowEmpty: true })
      .pipe(exec("ng serve"))
      .pipe(exec.reporter());
  MessageLoop("ng");
  return gulp;
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
    .src(".")
    .pipe(exec(`cd .. && dotnet restore`))
    .pipe(exec.reporter());
});

gulp.task("dotnet-run", function () {
  console.log("🚀 Starting .NET kestrel...");
  gulp
    .src(".")
    .pipe(exec(`cd .. && dotnet run`))
    .pipe(exec.reporter());
  MessageLoop("net");
  return gulp;
});

function MessageLoop(session){
  for (let i = 0; i <= 2; i++) {
    setTimeout(() => {
      if (i === 0) console.log("✅ Doing stuff .. ⏳");
      if (i === 1) console.log("✅ Success on stuff .. ✨");
      if(session === "net"){
        if (i === 2) console.log("✅ Up on http://localhost:7575 🚀");
      }
      if(session === "ng"){
        if (i === 2){
          console.log("✅ Client up on http://localhost:4200/ - With live edit 🚀");
        }
      }
    }, i * 2000);
  }
}

gulp.task(
  "build-angular-net",
  gulp.series(
    "ng-build",
    "ng-move-build",
    "ng-clean-build",
    "dotnet-restore",
    "dotnet-run"
  )
);

gulp.task(
  "build-angular",
  gulp.series(
    "ng-serve",
  )
);
