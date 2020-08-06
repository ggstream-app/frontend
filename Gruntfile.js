/// <binding BeforeBuild='build-deps' />
module.exports = function (grunt) {
    grunt.initConfig({
        clean: {
            dist: ['wwwroot/dist/*'],
            temp: ['temp/']
        },
        copy: {
            bootstrap_js: {
                files: [
                    {
                        src: 'node_modules/jquery/dist/jquery.js',
                        dest: 'temp/bootstrap_js/',
                        expand: true,
                        flatten: true,
                    },
                    {
                        src: 'node_modules/@popperjs/core/dist/umd/popper.js',
                        dest: 'temp/bootstrap_js/',
                        expand: true,
                        flatten: true,
                    },
                    {
                        src: 'node_modules/bootstrap/dist/js/bootstrap.js',
                        dest: 'temp/bootstrap_js/',
                        expand: true,
                        flatten: true,
                    },
                ]
            },
            argon_js: {
                files: [
                    {
                        src: 'node_modules/argon-design-system-free/assets/js/argon-design-system.js',
                        dest: 'temp/argon_js/',
                        expand: true,
                        flatten: true,
                    },
                    {
                        src: 'node_modules/argon-design-system-free/assets/js/plugins/datetimepicker.js',
                        dest: 'temp/argon_js/',
                        expand: true,
                        flatten: true,
                    },
                ]
            },
            validation_js: {
                files: [
                    {
                        src: 'node_modules/jquery-validation/dist/jquery.validate.js',
                        dest: 'temp/validation_js/',
                        expand: true,
                        flatten: true,
                    },
                    {
                        src: 'node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js',
                        dest: 'temp/validation_js/',
                        expand: true,
                        flatten: true,
                    },
                ]
            },
            ovenplayer_js: {
                files: [
                    {
                        src: 'node_modules/dashjs/dist/dash.all.debug.js',
                        dest: 'temp/ovenplayer_js/',
                        expand: true,
                        flatten: true,
                    },
                    {
                        src: 'node_modules/hls.js/dist/hls.js',
                        dest: 'temp/ovenplayer_js/',
                        expand: true,
                        flatten: true,
                    },
                    {
                        src: 'wwwroot/lib/ovenplayer/*.js',
                        dest: 'temp/ovenplayer_js/',
                        expand: true,
                        flatten: true,
                    },
                ]
            },
            css: {
                files: [
                    {
                        src: 'node_modules/bootstrap/dist/css/bootstrap.css',
                        dest: 'temp/css/0_bootstrap.css',
                    },
                    {
                        src: 'node_modules/argon-design-system-free/assets/css/argon-design-system.css',
                        dest: 'temp/css/1_argon.css',
                    },
                ]
            },
        },
        uglify: {
            js: {
                src: ['temp/jquery_js/*', 'temp/bootstrap_js/*', 'temp/argon_js/*'],
                dest: 'wwwroot/dist/deps.min.js',
            },
            validation: {
                src: ['temp/validation_js/*'],
                dest: 'wwwroot/dist/validation.min.js',
            },
            ovenplayer: {
                src: ['temp/ovenplayer_js/*'],
                dest: 'wwwroot/dist/ovenplayer.min.js',
            },
        },
        cssmin: {
            css: {
                src: ['temp/css/*'],
                dest: 'wwwroot/dist/deps.min.css',
            },
        },
    });

    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks("grunt-contrib-copy");
    grunt.loadNpmTasks("grunt-contrib-uglify");
    grunt.loadNpmTasks("grunt-contrib-cssmin");

    grunt.registerTask("build-deps", ['clean', 'copy', 'uglify', 'cssmin']);
};