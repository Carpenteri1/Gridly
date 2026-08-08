var gulp = require("gulp");
var { exec, spawn } = require('child_process');
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
    prom.rm("../bin", {recursive: true, force: true})
  ]);
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
  console.log("Starting .NET kestrel...");

  const child = spawn("dotnet", ["run"], {
    cwd: path.resolve(__dirname, ".."),
    stdio: "inherit",
    shell: false
  });

  child.on("error", (error) => {
    console.error(error);
  });

  MessageLoop("net");

  return child;
});

function MessageLoop(session) {
  for (let i = 0; i <= 2; i++) {
    setTimeout(() => {
      if (i === 0) console.log("Doing stuff ..");
      if (i === 1) console.log("Success on stuff ..");

      if (session === "net" && i === 2) {
        console.log("Up on http://localhost:7575");
      }

      if (session === "ng" && i === 2) {
        console.log("Client up on http://localhost:4200/ - With live edit");
      }
    }, i * 2000);
  }
}

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
