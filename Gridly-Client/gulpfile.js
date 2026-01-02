var gulp = require("gulp");
var gulpexec = require("gulp-exec");
var clean = require('gulp-clean');
var { exec, spawn } = require('child_process');
var ngProcess = null;

// ---- Angular ----
gulp.task('ng-build', function () {
  return gulp.src('./')
    .pipe(gulpexec('ng build --output-path=dist'))
    .pipe(gulpexec.reporter());
});

gulp.task("ng-serve", function (done) {
  console.log("🚀 Starting client server...");
  ngProcess = spawn('ng', ['serve'], {
    stdio: 'inherit',
    shell: true,
    cwd: process.cwd()
  });
  MessageLoop("ng");
      
  ngProcess.on('close', (code) => {
    if (code !== null) {
      console.log(`ng serve exited with code ${code}`);
      done();
    }
  });
  
  ngProcess.on('error', (err) => {
    console.error('Failed to start ng serve:', err);
    done(err);
  });

  done();
});


gulp.task("ng-stop", function (done) {
  console.log("🛑 Stopping ng serve...");
  
  if (ngProcess) {
    ngProcess.kill('SIGTERM');
    ngProcess = null;
  }
  
  const isWindows = process.platform === 'win32';
  
  if (isWindows) {
    exec('netstat -ano | findstr :4200', (error, stdout) => {
      if (stdout) {
        const lines = stdout.trim().split('\n');
        lines.forEach(line => {
          const parts = line.trim().split(/\s+/);
          const pid = parts[parts.length - 1];
          if (pid) {
            exec(`taskkill /F /PID ${pid}`, () => {});
          }
        });
      }
      done();
    });
  } else {
    exec('pkill -f "ng serve" || lsof -ti:4200 | xargs kill -9 2>/dev/null || true', (error) => {
      if (!error) {
        console.log("✅ ng serve stopped");
      }
      done();
    });
  }
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

// ---- dotnet ----
gulp.task("dotnet-restore", function () {
  return gulp
    .src(".")
    .pipe(gulpexec(`cd .. && dotnet restore`))
    .pipe(gulpexec.reporter());
});

gulp.task("dotnet-build-debug", function () {
  return gulp
    .src(".")
    .pipe(gulpexec(`cd .. && dotnet build --configuration Debug`))
    .pipe(gulpexec.reporter());
});

gulp.task("dotnet-run", function () {
  console.log("🚀 Starting .NET kestrel...");
  gulp
    .src(".")
    .pipe(gulpexec(`cd .. && dotnet run`))
    .pipe(gulpexec.reporter());
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
  "build-net-debug",
  gulp.series(
    "dotnet-restore",
    "dotnet-build-debug"
  )
);

gulp.task(
  "build-angular-net-debug",
  gulp.series(
    "ng-build",
    "ng-move-build",
    "ng-clean-build",
    "dotnet-build-debug"
  )
);

gulp.task(
  "build-angular-debug",
  gulp.series(
    "ng-serve",
  )
);
