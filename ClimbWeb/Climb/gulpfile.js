/// <binding ProjectOpened='watch-less' />
const gulp = require("gulp");
const ts = require("gulp-typescript");
const less = require("gulp-less");
const path = require("path");

var tsProject = ts.createProject('tsconfig.gulp.json');
gulp.task("ts", function () {
    return gulp.src('ClientApp/scripts/**/*.ts')
        .pipe(tsProject())
        .pipe(gulp.dest('wwwroot/dist/scripts'));
});

var tsClimbProject = ts.createProject('tsconfig.gulp.json');
gulp.task("ts-climb", function () {
    return gulp.src('ClientApp/gen/climbClient.ts')
        .pipe(tsClimbProject())
        .pipe(gulp.dest('wwwroot/dist/gen'));
});

gulp.task("less", () => {
    return gulp.src("ClientApp/styles/**/*.less")
        .pipe(less({
            paths: [path.join(__dirname, "ClientApp", "styles")]
        }))
        .pipe(gulp.dest("wwwroot/dist/styles"));
});

gulp.task("watch-less", () => {
    gulp.watch("./ClientApp/styles/**/*.less", gulp.series("less"));
});

gulp.task("default", gulp.parallel("less", "ts", "ts-climb"));