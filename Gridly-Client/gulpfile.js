var gulp = require("gulp");
var { spawn } = require('child_process');
var ngProcess = null;
const prom= require("fs/promises");
const path = require("path");


// ---- Angular ----
function runCommand(command, args, options = {}) {
  return new Promise((resolve, reject) => {
    const child = spawn(command, args, {
      cwd: __dirname,
      stdio: "inherit",
      shell: false,
      ...options
    });

    child.on("error", reject);

    child.on("close", (code) => {
      if (code === 0) {
        resolve();
      } else {
        reject(new Error(`${command} ${args.join(" ")} failed with code ${code}`));
      }
    });
  });
}

gulp.task("ng-build", async function () {
  const ng = process.platform === "win32"
    ? path.resolve(__dirname, "node_modules/.bin/ng.cmd")
    : path.resolve(__dirname, "node_modules/.bin/ng");

  await runCommand(ng, ["build"]);
});
gulp.task("clean-build", async function () {
  await Promise.all([
    prom.rm("../wwwroot", {recursive: true, force: true}),
    prom.rm("../bin", {recursive: true, force: true}),
    prom.rm("../publish", {recursive: true, force: true})
  ]);
});


gulp.task("ng-serve", function (done) {
  ngProcess = spawn('ng', ['serve'], {
    stdio: 'inherit',
    shell: true,
    cwd: process.cwd()
  });

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

gulp.task("ng-move-build", async function () {
  const source = path.resolve(__dirname, "../wwwroot/browser");
  const target = path.resolve(__dirname, "../wwwroot");

  await prom.cp(source, target, {
    recursive: true,
    force: true
  });

  await prom.rm(source, {
    recursive: true,
    force: true
  });
});

// ---- dotnet ----
gulp.task("dotnet-build", async function () {
  await runCommand("dotnet", ["build"], {
    cwd: path.resolve(__dirname, "..")
  });
});

gulp.task("dotnet-run", function () {
  const child = spawn("dotnet", ["run", "--configuration", "Debug"], {
    cwd: path.resolve(__dirname, ".."),
    stdio: "inherit",
    shell: false
  });

  child.on("error", (error) => {
    console.error(error);
  });

  return child;
});

/**
 * Publish for test
 * Pi: linux-arm64
 * Windows: win-x64
 * Mac: osx-arm64
 * Linux: linux-x64
 * Put in Gulp argument
 * */

gulp.task("publish-dotnet", async function () {
   let runtime =
     {
      pi: "linux-arm64",
      windows: "win-x64",
      mac: "osx-arm64",
      linux: "linux-x64",
      raspberry: "linux-arm64",
    };
  await publishDotnet(runtime.mac);
});

async function publishDotnet(runtime) {
  await runCommand("dotnet", [
    "publish",
    "Gridly.csproj",
    "--configuration", "Release",
    "--runtime", runtime,
    "--self-contained", "true",
    "--output", path.resolve(__dirname, `../publish/${runtime}`)
  ], {
    cwd: path.resolve(__dirname, "..")
  });
}

//Tasks and series

gulp.task(
  "build-net",
  gulp.series(
    "clean-build",
    "dotnet-build"
  )
);

gulp.task(
  "build-angular",
  gulp.series(
    "clean-build",
    "ng-build",
    "ng-move-build"
  )
);

gulp.task(
  "run-angular",
  gulp.series(
    "clean-build",
    "ng-serve",
  )
);

gulp.task(
  "run-net-angular",
  gulp.series(
    "clean-build",
    "ng-build",
    "ng-move-build",
    "dotnet-run"
  )
);

gulp.task(
  "publish",
  gulp.series(
    'clean-build',
    'ng-build',
    'ng-move-build',
    'publish-dotnet',
  )
);
