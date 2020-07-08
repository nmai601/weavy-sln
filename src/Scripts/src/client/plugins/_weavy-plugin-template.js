﻿/* eslint-env commonjs, amd */

// UMD based on https://github.com/umdjs/umd/blob/master/templates/returnExports.js
// TODO: move to ES6 and transpiler

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define([
            'jquery',
            'weavy'
        ], factory);
    } else if (typeof module === 'object' && module.exports) {
        // Node. Does not work with strict CommonJS, but
        // only CommonJS-like environments that support module.exports,
        // like Node.
        module.exports = factory(
            require('jquery'),
            require('weavy')
        );
    } else {
        // Browser globals (root is window)
        if (typeof Weavy === 'undefined' || !Weavy.plugins) {
            throw new Error("Weavy must be loaded before registering plugin");
        }

        factory(jQuery, Weavy);
    }
}(typeof self !== 'undefined' ? self : this, function ($, Weavy) {

    /**
     * Describe your plugin.
     * 
     * @mixin MyPlugin
     * @returns {Weavy.plugins.myplugin}
     * @typicalname weavy
     */
    var MyPlugin = function (options) {
        /**
         * The nodes placeholder in [Weavy]{@link Weavy#nodes}
         * @instance
         * @member nodes
         * @memberof MyPlugin
         * @extends Weavy#nodes
         * @typicalname weavy.nodes
         */

        /**
         *  Reference to this instance
         *  @lends MyPlugin#
         */
        var weavy = this;

        // MY CUSTOM CODE
        // ...

        // Exports
        return {}
    };

    /**
     * Default plugin options
     * 
     * @example
     * Weavy.plugins.MyPlugin.defaults = {
     * };
     * 
     * @name defaults
     * @memberof MyPlugin
     * @type {Object}
     */
    MyPlugin.defaults = {
    };

    /**
     * Non-optional dependencies.
     * 
     * @name dependencies
     * @memberof MyPlugin
     * @type {string[]}
     */
    MyPlugin.dependencies = [];


    // Register and return plugin
    console.debug("Registering Weavy plugin: myplugin");
    return Weavy.plugins.myplugin = MyPlugin;

}));
