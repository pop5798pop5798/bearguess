;(function($) {
    'use strict';
     

    var $elements = null,
        elementsArr,
        animationsArr,
        scroll,
        windowHeight, windowWidth,
        documentWidth, documentHeight,
        scrollTicking = false,
        resizeTicking = false,
        isTouchDevice = window.Modernizr && typeof(Modernizr.touchevents) != 'undefined' ? Modernizr.touchevents : testTouchEvents(),
        PERC_RE = /%/g,
        VU_RE = /v(w|h)/g;
    
    $.parallax = {
        enableTouchDevices: false
    };

    function testTouchEvents() {
        return ('ontouchstart' in window) ||
            (navigator.maxTouchPoints > 0) ||
            (navigator.msMaxTouchPoints > 0);
    }

    $.fn.parallax = function(method) {
        switch (method) {
            case 'reset':
                // todo: implement this
                //this.css('transform', '');
                break;
            case 'destroy':
                // todo: implement this
                $elements.not(this);
                break;
            default:
                if (!isTouchDevice || $.parallax.enableTouchDevices || (method && method.enableTouchDevices)) {
                    this.data("parallax-js", method);
                    var firstCall = ($elements === null);
                    if (firstCall) {
                        updateDimensions();
                    }
                    if (firstCall) {
                        $elements = this;
                        window.onresize = onResize;
                        window.onscroll = onScroll;
                        elementsArr = [];
                        animationsArr = [];
                    }
                    else {
                        $elements = $elements.add(this);
                    }
                    updateAnimationsArray.call(this, elementsArr.length);
                    elementsArr = $elements.toArray();
                    onScroll();
                }
        }
        return this;
    };

    function parseOptions() {
        var optionsArr = [],
            dataOptions = this.data("parallax"),
            jsOptions = this.data("parallax-js");
        typeof dataOptions != "undefined" || (dataOptions = {});
        typeof dataOptions == "object" || console.error("Unable to parse data-parallax attribute "+getSelector(this));
        typeof jsOptions != "undefined" || (jsOptions = {});
        typeof jsOptions == "object" || console.error("Unrecognized options passed to $.fn.parallax");
        if (!Array.isArray(dataOptions)) {
            dataOptions = [dataOptions];
        }
        if (!Array.isArray(jsOptions)) {
            jsOptions = [jsOptions];
        }
        for (var i= 0, len = Math.max(dataOptions.length, jsOptions.length); i<len; i++) {
            var options = $.extend(dataOptions[i] || {}, jsOptions[i] || {});
            
            // todo: remove in next minor release
            typeof options.start === "undefined" || (options.triggerElement = options.start);
            typeof options.trigger == "undefined" || (options.triggerHook = options.trigger);
            
            typeof options.offset != "undefined" || (options.offset = 0);
            typeof options.triggerElement === "undefined" || (options.triggerElement = convertToElement(options.triggerElement));
            typeof options.triggerElement != "undefined" || (options.triggerElement = this[0]);
            optionsArr.push(options);
        }
        return optionsArr;
    }
    
    function rebuildAnimationsArray() {
        animationsArr = [];
        PinScene.scenes = [];
        updateAnimationsArray.call($elements);
    }

    function updateAnimationsArray(offset) {
        typeof offset === "number" || (offset = 0);
        this.each(function(i) {
            var idx = offset+i;
            animationsArr[idx] = createAnimations.call(this);
        });
    }

    function createAnimations() {
        var $this = $(this),
            animations = [],
            optionsArr = parseOptions.call($this);
        for (var i= 0, len = optionsArr.length; i<len; i++) {
            var options = optionsArr[i],
                globalOptions = {
                    axis: options.axis,
                    triggerElement: options.triggerElement,
                    triggerHook: options.triggerHook,
                    duration: options.duration,
                    offset: options.offset
                },
                animation = {},
                transformOptions = {},
                bgPositionOptions = {};
            if (typeof options.x != "undefined") {
                transformOptions.x = mergeOptions(options.x, globalOptions);
            }
            if (typeof options.y != "undefined") {
                transformOptions.y = mergeOptions(options.y, globalOptions);
            }
            if (typeof options.z != "undefined") {
                transformOptions.z = mergeOptions(options.z, globalOptions);
            }
            if (typeof options.scale != "undefined") {
                transformOptions.scale = mergeOptions(options.scale, globalOptions);
            }
            else {
                if (typeof options.scaleX != "undefined") {
                    transformOptions.scaleX = mergeOptions(options.scaleX, globalOptions);
                }
                if (typeof options.scaleY != "undefined") {
                    transformOptions.scaleY = mergeOptions(options.scaleY, globalOptions);
                }
            }
            if (typeof options.rotate != "undefined") {
                transformOptions.rotate = mergeOptions(options.rotate, globalOptions);
            }
            if (transformOptions.x || transformOptions.y || transformOptions.z || 
                transformOptions.scale || transformOptions.scaleX || transformOptions.scaleY || 
                transformOptions.rotate) {
                animation.transform = new TransformContainer($this, transformOptions);
            }

            if (typeof options.backgroundPositionX != "undefined") {
                bgPositionOptions.x = mergeOptions(options.backgroundPositionX, globalOptions);
            }
            if (typeof options.backgroundPositionY != "undefined") {
                bgPositionOptions.y = mergeOptions(options.backgroundPositionY, globalOptions);
            }
            if (bgPositionOptions.x || bgPositionOptions.y) {
                animation.bgPosition = new XYContainer($this, bgPositionOptions, 'backgroundPosition');
            }

            if (typeof options.top != "undefined") {
                var topOptions = mergeOptions(options.top, globalOptions);
                animation.top = new StyleScene($this, topOptions, 'top', $this.offsetParent().height(), "px");
            }
            if (typeof options.left != "undefined") {
                var leftOptions = mergeOptions(options.left, globalOptions);
                animation.left = new StyleScene($this, leftOptions, 'left', $this.offsetParent().width(), "px");
            }
            if (typeof options.width != "undefined") {
                var widthOptions = mergeOptions(options.width, globalOptions);
                animation.width = new StyleScene($this, widthOptions, 'width');
            }
            if (typeof options.height != "undefined") {
                var heightOptions = mergeOptions(options.height, globalOptions);
                animation.height = new StyleScene($this, heightOptions, 'height');
            }
            if (typeof options.opacity != "undefined") {
                var opacityOptions = mergeOptions(options.opacity, globalOptions);
                animation.opacity = new StyleScene($this, opacityOptions, 'opacity', 1);
            }
            if (typeof options.color != "undefined") {
                var colorOptions = mergeOptions(options.color, globalOptions);
                animation.color = new ColorScene($this, colorOptions, 'color', 0xffffff);
            }
            if (typeof options.backgroundColor != "undefined") {
                var bgColorOptions = mergeOptions(options.backgroundColor, globalOptions);
                animation.bgColor = new ColorScene($this, bgColorOptions, 'backgroundColor', 0xffffff);
            }
            if (typeof options.pin != "undefined") {
                var pinOptions = mergeOptions(options.pin, globalOptions);
                animation.pin = new PinScene($this, pinOptions);
            }
            if (typeof options.class != "undefined") {
                var classOptions = mergeOptions(options.class, globalOptions);
                animation.class = new ClassScene($this, classOptions);
            }
            animations.push(animation);
        }
        return animations;
    }

    function onResize() {
        if (!resizeTicking) {
            window.requestAnimationFrame(function() {
                updateDimensions();
                rebuildAnimationsArray();
            });
            resizeTicking = true;
        }
    }

    function updateDimensions() {
        var body = document.body,
            html = document.documentElement;

        windowWidth = Math.max(html.clientWidth, window.innerWidth || 0);
        windowHeight = Math.max(html.clientHeight, window.innerHeight || 0);

        documentWidth = Math.max( body.scrollWidth, body.offsetWidth,
            html.clientWidth, html.scrollWidth, html.offsetWidth );
        documentHeight = Math.max( body.scrollHeight, body.offsetHeight,
            html.clientHeight, html.scrollHeight, html.offsetHeight );
    }

    function onScroll() {
        if (!scrollTicking) {
            window.requestAnimationFrame(animateElements);
            scrollTicking = true;
        }
    }

    function animateElements() {
        scroll = getScroll();
        for (var i= 0, len=elementsArr.length; i<len; i++) {
            animateElement.call(elementsArr[i], i);
        }
        scrollTicking = false;
    }
    
    function getScroll() {
        return {
            left: window.pageXOffset || document.documentElement.scrollLeft,
            top: window.pageYOffset || document.documentElement.scrollTop
        };
    }

    function animateElement(idx) {
        var animations = animationsArr[idx],
            animation,
            style;
        typeof window.getComputedStyle != "function" || (style = getComputedStyle(this));
        for (var i=0, len=animations.length; i<len; i++) {
            animation = animations[i];
            for (var name in animation) {
                if (animation[name].needsUpdate()) {
                    animation[name].update(style);
                }
            }
        }
    }

    function mergeOptions(options, globalOptions) {
        if (typeof options != "object") {
            options = {to: options};
        }
        return $.extend({}, globalOptions, options);
    }

    function getOffset(elem) {
        var offsetLeft = elem.offsetLeft,
            offsetTop = elem.offsetTop,
            lastElem = elem;
        while (elem = elem.offsetParent) {
            if (elem === document.body) { //from my observation, document.body always has scrollLeft/scrollTop == 0
                break;
            }
            offsetLeft += elem.offsetLeft;
            offsetTop += elem.offsetTop;
            lastElem = elem;
        }
        if (lastElem.style.position === 'fixed') { //slow - http://jsperf.com/offset-vs-getboundingclientrect/6
            offsetLeft += scroll.left;
            offsetTop += scroll.top;
        }
        return {
            left: offsetLeft,
            top: offsetTop
        };
    }
    
    function convertToOffset(elem, axis) {
        return getOffset(elem)[axis === Scene.AXIS_X ? 'left' : 'top'];
    }

    function convertToElement(value) {
        if (typeof value === "string") {
            value = $(value);
            if (value.length) {
                return value[0];
            }
            console.error("Invalid parallax triggerElement selector: "+value);
        }
        else {
            return value;
        }
    }

    function convertOption(value, maxValue) {
        if (typeof value === "string") {
            if (value.match(PERC_RE)) {
                value = convertPerc(value, maxValue);
            }
            else {
                var matches = value.match(VU_RE);
                if (matches) {
                    value = convertVU(value, matches[0]);
                }
            }
        }
        else if (typeof value === "function") {
            value = value(maxValue);
        }
        return value;
    }
    
    function convertVU(percent, unit) {
        return convertPerc(percent, (unit === 'vw' ? windowWidth : windowHeight));
    }

    function convertPerc(percent, maxValue) {
        return parseFloat(percent) / 100 * maxValue;
    }

    function isElement(obj) {
        try {
            return obj instanceof HTMLElement;
        }
        catch(e) {
            return (typeof obj === "object") && (obj.nodeType === 1) &&
                (typeof obj.style === "object") && (typeof obj.ownerDocument ==="object");
        }
    }
    
    function interpolate(from, to, progress) {
        return (to - from) * progress + from;
    }
    
    function inherit(parentProto, childProto) {
        return $.extend(Object.create(parentProto), childProto || {});
    }
    
    function getSelector($el) {
        var selector = "",
            id = $el.attr("id"),
            classNames = $el.attr("class");
        if (id) {
            selector += "#"+ id;
        }
        else if (classNames) {
            selector += "." + $.trim(classNames).replace(/\s/gi, ".");
        }
        return selector;
    }
    
    function parseUnit(value) {
        return value.replace(/^-?\d+(\.\d*)?(\D+)$/, "$2");
    }
    
    function Scene($el, options) {
        this.$el = $el;
        this.from = options.from;
        this.to = options.to;
        this.axis = options.axis;
        
        this.offset = convertOption(options.offset, this.getElementDimension());
        
        typeof options.triggerHook != "undefined" || (options.triggerHook = "100%");
        this.triggerHook = convertOption(options.triggerHook, options.axis === Scene.AXIS_X ? windowWidth : windowHeight);
        this.triggerElement = convertToElement(options.triggerElement);
        
        this._setEase(options.ease);
        this._setDuration(options.duration);
    }
    Scene.AXIS_X = 'x';
    Scene.AXIS_Y = 'y';
    Scene.STATE_BEFORE = 'before';
    Scene.STATE_DURING = 'during';
    Scene.STATE_AFTER = 'after';
    Scene.prototype = {
        _setEase: function(ease) {
            if (typeof ease == "function") {
                this.ease = ease;
            }
            else {
                typeof ease === "undefined" || (this.ease = $.easing[ease]);
                typeof this.ease === "function" || (this.ease = $.easing.linear);
            }
        },
        _setDuration: function(duration) {
            var validateDurationPx = function(value) {
                if (value < 0) {
                    console.error("Invalid parallax duration: "+value);
                }
            };
            if (typeof duration === "undefined") {
                var scene = this;
                this.duration = function() {
                    var durationPx = (convertToOffset(scene.$el[0], scene.axis) + scene.$el.outerHeight(true)) - scene.start;
                    validateDurationPx(durationPx);
                    return durationPx;
                };
            }
            else if (typeof duration === "function") {
                this.duration = function() {
                    var durationPx = duration();
                    validateDurationPx(durationPx);
                    return durationPx;
                };
            }
            else {
                var durationPx = convertOption(duration, this.getElementDimension());
                validateDurationPx(durationPx);
                this.duration = function () {
                    return durationPx;
                };
            }
        },
        getElementDimension: function() {
            return this.axis === Scene.AXIS_X ? 
                this.$el.outerWidth(true) : 
                this.$el.outerHeight(true);
        },
        needsUpdate: function() {
            this.updateStart();
            this.updateDuration();
            this.updateState();
            return this._needsUpdate();
        },
        _needsUpdate: function() {
            return this.state === Scene.STATE_DURING ||
                (typeof this.prevState === "undefined" && this.state === Scene.STATE_AFTER) ||
                (typeof this.prevState != "undefined" && this.prevState != this.state);
        },
        updateStart: function() {
            this.start = Math.max(this.getOffset() - this.triggerHook, 0);
        },
        updateDuration: function() {
            this.durationPx = this.duration.call(this);
            if (this.durationPx === 0) {
                this.durationPx = ((this.axis === Scene.AXIS_X ? 
                    documentWidth - windowWidth : 
                    documentHeight - windowHeight) - this.start);
            }
        },
        updateState: function() {
            this.prevState = this.state;
            if (scroll.top < this.start) {
                this.state = Scene.STATE_BEFORE;
            }
            else if (scroll.top <= (this.start + this.durationPx)) {
                this.state = Scene.STATE_DURING;
            }
            else {
                this.state = Scene.STATE_AFTER;
            }
        },
        getOffset: function() {
            var offset = this.offset;
            if (isElement(this.triggerElement)) {
                var pinScene = PinScene.findByElement(this.triggerElement);
                offset += (pinScene && pinScene.state === Scene.STATE_DURING ? 
                    pinScene.start : 
                    convertToOffset(this.triggerElement, this.axis));
            }
            return offset;
        },
        getProgress: function() {
            if (this.state === Scene.STATE_BEFORE) {
                return 0;
            }
            else if (this.state === Scene.STATE_DURING) {
                var posPx = scroll.top - this.start,
                    percent = posPx / this.durationPx,
                    progress = this.ease.call(this, percent);
                return progress;
            }
            else {
                return 1;
            }
        },
        update: function(style) {
            this._setFrom(this._getOldValue(style));
            this._setValue(this._getNewValue(), style);
        },
        _getOldValue: function() {},
        _getNewValue: function() {},
        _setFrom: function(defaultValue) {
            typeof this.from != "undefined" || (this.from = defaultValue);
        }
    };
    
    function ScalarScene($el, options, maxValue, defaultUnit) {
        this.convertPerc = false;
        this.unit = defaultUnit;
        if (typeof maxValue != "undefined") {
            options.from = convertOption(options.from, maxValue);
            options.to = convertOption(options.to, maxValue);
        }
        else {
            if (typeof options.from === "string") {
                options.from = this._parseString(options.from);
            }
            else if (typeof options.from === "function") {
                options.from = options.from();
            }
            if (typeof options.to === "string") {
                options.to = this._parseString(options.to);
            }
            else if (typeof options.to === "function") {
                options.to = options.to();
            }
        }
        Scene.call(this, $el, options);
    }
    ScalarScene.prototype = inherit(Scene.prototype, {
        _parseString: function(value) {
            if (value.match(PERC_RE)) {
                this.convertPerc = true;
            }
            else {
                var matches = value.match(VU_RE);
                if (matches) {
                    value = convertVU(value, matches[0]);
                }
                else {
                    this.unit = parseUnit(value);
                }
            }
            return value;
        },
        _getNewValue: function() {
            var from = this.from,
                to = this.to;
            if (typeof from === "string") {
                if (this.convertPerc && from.substr(-1) === "%") {
                    from = convertOption(from, this.durationPx);
                }
                else {
                    from = parseFloat(from);
                }
            }
            if (typeof to === "string") {
                if (this.convertPerc && to.substr(-1) === "%") {
                    to = convertOption(to, this.durationPx);
                }
                else {
                    to = parseFloat(to);
                }
            }
            var suffix = (typeof this.unit === "undefined" ? 0 : this.unit);
            return interpolate(from, to, this.getProgress()) + suffix;
        }
    });
    
    function StyleScene($el, options, styleName, maxValue, defaultUnit) {
        this.styleName = styleName;
        ScalarScene.call(this, $el, options, maxValue, defaultUnit);
    }
    StyleScene.prototype = inherit(ScalarScene.prototype, {
        _getOldValue: function(style) {
            return parseFloat(style[this.styleName]);
        },
        _setValue: function(newValue) {
            this.$el[0].style[this.styleName] = newValue;
        }
    });
    
    function ColorScene($el, options, styleName, maxValue) {
        StyleScene.call(this, $el, options, styleName, maxValue);
    }
    ColorScene.prototype = inherit(StyleScene.prototype, {
        _getOldValue: function(style) {
            return style[this.styleName];
        },
        _getNewValue: function() {
            var fromColor = RGBColor.fromString(this.from),
                toColor = RGBColor.fromString(this.to);
            fromColor.interpolate(toColor, this.getProgress());
            return fromColor.toString();
        }
    });

    function StateScene($el, options) {
        typeof options.triggerHook != "undefined" || (options.triggerHook = 0);
        Scene.call(this, $el, options);
    }
    StateScene.prototype = inherit(Scene.prototype, {
        _needsUpdate: function() {
            return (typeof this.prevState != "undefined" || this.state == Scene.STATE_DURING) &&
                this.prevState != this.state;
        }
    });

    function ClassScene($el, options) {
        StateScene.call(this, $el, options);
    }
    ClassScene.prototype = inherit(Scene.prototype, {
        _setValue: function() {
            this.$el[this.state == Scene.STATE_DURING ? 'addClass' : 'removeClass'](this.to);
        }
    });

    function PinScene($el, options) {
        options.to = convertToElement(options.to);
        isElement(options.to) || (options.to = $el[0]);
        typeof options.triggerHook != "undefined" || (options.triggerHook = 0);
        StateScene.call(this, $el, options);
        PinScene.scenes.push(this);
    }
    PinScene.scenes = [];
    PinScene.findByElement = function(elem) {
        var scenes = PinScene.scenes;
        for (var i=0, len=scenes.length; i<len; i++) {
            if (scenes[i].$el[0] === elem) {
                return scenes[i];
            }
        }
    };
    PinScene.prototype = inherit(StateScene.prototype, {
        updateStart: function() {
            if (this.state != Scene.STATE_DURING) {
                Scene.prototype.updateStart.call(this);
            }
        },
        _getOldValue: function(style) {
            var toStyle = getComputedStyle(this.to);
            return {
                position: toStyle.position,
                top: toStyle.top,
                left: toStyle.left,
                marginLeft: "",
                marginTop: ""
            };
        },
        _getNewValue: function() {
            if (this.state == Scene.STATE_DURING) {
                return {
                    position: 'fixed',
                    top: this.from.pinTop + 'px',
                    left: this.from.pinLeft + 'px',
                    marginLeft: 0,
                    marginTop: 0
                };
            }
            return this.from;
        },
        _setValue: function(newValue) {
            for (var styleName in newValue) {
                this.to.style[styleName] = newValue[styleName];                
            }
        },
        _setFrom: function(defaultValue) {
            if (typeof this.from === "undefined") {
                var offset = getOffset(this.to);
                if (this.axis === Scene.AXIS_X) {
                    defaultValue.pinTop = offset.top;
                    defaultValue.pinLeft = offset.left - this.start;
                }
                else {
                    defaultValue.pinTop = offset.top - this.start;
                    defaultValue.pinLeft = offset.left;
                }
                this.from = defaultValue;
            }
        }
    });
    
    function VOScene($el, options, propName, maxValue) {
        this.propName = propName;
        ScalarScene.call(this, $el, options, maxValue);
    }
    VOScene.prototype = inherit(ScalarScene.prototype, {
        _getOldValue: function(vo) {
            return vo.get(this.propName);
        },
        _setValue: function(newValue, vo) {
            vo.set(this.propName, newValue);
        }
    });
    
    function SceneContainer($el, options) {
        this.$el = $el;
    }
    SceneContainer.prototype = {
        needsUpdate: function() {
            return true;
        }
    };
    
    function XYContainer($el, options, styleName) {
        SceneContainer.call(this, $el, options);
        this.styleName = styleName;
        if (options.x) {
            this.x = new VOScene($el, options.x, 'x');
        }
        if (options.y) {
            this.y = new VOScene($el, options.y, 'y');
        }
    }
    XYContainer.prototype = inherit(SceneContainer.prototype, {
        update: function(style) {
            var xy = XY.fromString(style[this.styleName]);
            if (this.x && this.x.needsUpdate()) {
                this.x.update(xy);
            }
            if (this.y && this.y.needsUpdate()) {
                this.y.update(xy);
            }
            if (xy.isChanged()) {
                var element = this.$el[0],
                    newValue = xy.toString();
                element.style[this.styleName] = newValue;
            }
        }
    });

    function TransformContainer($el, options) {
        SceneContainer.call(this, $el, options);
        if (options.x) {
            this.x = new VOScene($el, options.x, 'translateX');
        }
        if (options.y) {
            this.y = new VOScene($el, options.y, 'translateY');
        }
        if (options.z) {
            this.z = new VOScene($el, options.z, 'translateZ');
        }
        if (options.scale) {
            this.scale = new VOScene($el, options.scale, 'scale', 1);
        }
        else {
            if (options.scaleX) {
                this.scaleX = new VOScene($el, options.scaleX, 'scaleX', 1);
            }
            if (options.scaleY) {
                this.scaleY = new VOScene($el, options.scaleY, 'scaleY', 1);
            }
        }
        if (options.rotate) {
            this.rotate = new VOScene($el, options.rotate, 'rotate', 360);
        }
    }
    TransformContainer.prototype = inherit(SceneContainer.prototype, {
        update: function(style) {
            var matrix = TransformMatrix.fromStyle(style),
                transform = Transform.fromMatrix(matrix);
            if (this.x && this.x.needsUpdate()) {
                this.x.update(transform);
            }
            if (this.y && this.y.needsUpdate()) {
                this.y.update(transform);
            }
            if (this.z && this.z.needsUpdate()) {
                this.z.update(transform);
            }
            if (this.scale && this.scale.needsUpdate()) {
                this.scale.update(transform);
            }
            if (this.scaleX && this.scaleX.needsUpdate()) {
                this.scaleX.update(transform);
            }
            if (this.scaleY && this.scaleY.needsUpdate()) {
                this.scaleY.update(transform);
            }
            if (this.rotate && this.rotate.needsUpdate()) {
                this.rotate.update(transform);
            }
            if (transform.isChanged()) {
                var element = this.$el[0],
                    newValue = transform.toString();
                element.style['-webkit-transform'] = newValue;
                element.style['-moz-transform'] = newValue;
                element.style['-ms-transform'] = newValue;
                element.style['-o-transform'] = newValue;
                element.style.transform = newValue;
            }
        }
    });
    
    function RGBColor(r, g, b, a) {
        this.r = r || 0;
        this.g = g || 0;
        this.b = b || 0;
        this.a = typeof a === "number" ? a : 1;
    }
    RGBColor.fromArray = function(array, result) {
        result || (result = new RGBColor());
        if (array.length < 3) {
            return result;
        }
        result.r = parseInt(array[0]);
        result.g = parseInt(array[1]);
        result.b = parseInt(array[2]);
        if (array.length > 3) {
            result.a = parseFloat(array[3]);
        }
        return result;
    };
    RGBColor.fromString = function(string, result) {
        if (string.match(/^#([0-9a-f]{3})$/i)) {
            return RGBColor.fromArray([
                parseInt(string.charAt(1),16)*0x11,
                parseInt(string.charAt(2),16)*0x11,
                parseInt(string.charAt(3),16)*0x11
            ], result);
        }
        if (string.match(/^#([0-9a-f]{6})$/i)) {
            return RGBColor.fromArray([
                parseInt(string.substr(1,2),16),
                parseInt(string.substr(3,2),16),
                parseInt(string.substr(5,2),16)
            ], result);
        }
        return RGBColor.fromArray(string.replace(/^rgb(a)?\((.*)\)$/, '$2').split(","), result);
    };
    RGBColor.fromHSV = function(hsv, result) {
        result || (result = new RGBColor());
        var r = hsv.v,
            g = hsv.v,
            b = hsv.v;
        if (hsv.s != 0) {
            var f  = hsv.h / 60 - Math.floor(hsv.h / 60);
            var p  = hsv.v * (1 - hsv.s / 100);
            var q  = hsv.v * (1 - hsv.s / 100 * f);
            var t  = hsv.v * (1 - hsv.s / 100 * (1 - f));
            switch (Math.floor(hsv.h / 60)){
                case 0: r = hsv.v; g = t; b = p; break;
                case 1: r = q; g = hsv.v; b = p; break;
                case 2: r = p; g = hsv.v; b = t; break;
                case 3: r = p; g = q; b = hsv.v; break;
                case 4: r = t; g = p; b = hsv.v; break;
                case 5: r = hsv.v; g = p; b = q; break;
            }
        }
        result.r = r * 2.55;
        result.g = g * 2.55;
        result.b = b * 2.55;
        result.a = hsv.a;
        return result;
    };
    RGBColor.prototype = {
        getHue: function(maximum, range) {
            var hue = 0;
            if (range != 0) {
                switch (maximum){
                    case this.r:
                        hue = (this.g - this.b) / range * 60;
                        if (hue < 0) hue += 360;
                        break;
                    case this.g:
                        hue = (this.b - this.r) / range * 60 + 120;
                        break;
                    case this.b:
                        hue = (this.r - this.g) / range * 60 + 240;
                        break;
                }
            }
            return hue;
    
        },
        interpolate: function(to, progress) {
            var src = HSVColor.fromRGB(this),
                dst = HSVColor.fromRGB(to);
            src.interpolate(dst, progress);
            RGBColor.fromHSV(src, this);
        },
        toString: function() {
            if (this.a !== 1) {
                return "rgba("+this.r.toFixed()+","+this.g.toFixed()+","+this.b.toFixed()+","+this.a.toFixed(2)+")";
            }
            return "rgb("+this.r.toFixed()+","+this.g.toFixed()+","+this.b.toFixed()+")";
        }
    };
    
    function HSVColor(h, s, v, a) {
        this.h = h || 0;
        this.s = s || 0;
        this.v = v || 0;
        this.a = typeof a === "number" ? a : 1;
    }
    HSVColor.fromRGB = function(rgb, result) {
        result || (result = new HSVColor());
        var maximum = Math.max(rgb.r, rgb.g, rgb.b);
        var range   = maximum - Math.min(rgb.r, rgb.g, rgb.b);
        result.h = rgb.getHue(maximum, range);
        result.s = (maximum == 0 ? 0 : 100 * range / maximum);
        result.v = maximum / 2.55;
        result.a = rgb.a;
        return result;
    };
    HSVColor.prototype = {
        interpolate: function(to, progress, precision) {
            this.h = interpolate(this.h, to.h, progress);
            this.s = interpolate(this.s, to.s, progress);
            this.v = interpolate(this.v, to.v, progress);
            this.a = interpolate(this.a, to.a, progress);
        },
        toString: function() {
            if (this.a !== 1) {
                return "hsva("+this.h+","+this.s+","+this.v+","+this.a.toFixed(2)+")";
            }
            return "hsv("+this.h+","+this.s+","+this.v+")";
        }
    };
    
    function VO() {
        
    }
    VO.prototype = {
        get: function(propName) {
            return this[propName];
        },
        set: function(propName, value) {
            this[propName] = value;
            this._changed = true;
        },
        isChanged: function() {
            return this._changed === true;
        }
    };
    
    function XY() {
        this.x = this.y = 0;
        this.xUnit = this.yUnit = "px";
    }
    XY.fromArray = function(array, result) {
        result || (result = new XY());
        var a = array[0],
            b = array[1];
        if (typeof a === "string") {
            result.x = parseFloat(a);
            result.xUnit = parseUnit(a);
        }
        else {
            result.x = a;
        }
        if (typeof b === "string") {
            result.y = parseFloat(b);
            result.yUnit = parseUnit(b);
        }
        else {
            result.y = b;
        }
        return result;
    };
    XY.fromString = function(string, result) {
        return XY.fromArray(string.split(" "), result);
    };
    XY.prototype = inherit(VO.prototype, {
        toString: function() {
            return this.x.toFixed(2) + this.xUnit + " " + this.y.toFixed(2) + this.yUnit;
        }
    });

    function Transform() {
        this.translateX = this.translateY = this.translateZ = 0;
        this.scaleX = this.scaleY = 1;
        this.rotate = 0;
    }
    Transform.fromMatrix = function(matrix, result) {
        result || (result = new Transform());
        var a = matrix.matrix[0],
            b = matrix.matrix[1],
            c = matrix.matrix[4],
            d = matrix.matrix[5];
        result.translateX = matrix.matrix[12];
        result.translateY = matrix.matrix[13];
        result.translateZ = matrix.matrix[14];
        result.scaleX = Math.sqrt(a*a + b*b);
        result.scaleY = Math.sqrt(c*c + d*d);
        result.rotate = Math.round(Math.atan2(b, a) * (180/Math.PI));
        return result;
    };
    Transform.prototype = inherit(VO.prototype, {
        get: function(propName) {
            if (propName === "scale") {
                return this.scaleX;
            }
            return this[propName];
        },
        set: function(propName, value) {
            if (propName === "scale") {
                this.scaleX = value;
                this.scaleY = value;
            }
            else {
                this[propName] = value;
            }
            this._changed = true;
        },
        toString: function() {
            var string = 'translate3d('+this.translateX+'px, '+this.translateY+'px, '+this.translateZ+'px)';
            if (this.scaleX != 1 || this.scaleY != 1) {
                string += ' scale('+this.scaleX+','+this.scaleY+')';
            }
            if (this.rotate) {
                string += 'rotate('+this.rotate+'deg)';
            }
            return string;
        }
    });

    function TransformMatrix() {
        this.matrix = [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];
    }
    TransformMatrix.fromArray = function(array, result) {
        result || (result = new TransformMatrix());
        if (array.length < 6) {
            return result;
        }
        for (var i=0; i<array.length; i++) {
            array[i] = parseFloat(array[i]);
        }
        if (array.length < 16) {
            array = [
                array[0], array[1], 0, 0,
                array[2], array[3], 0, 0,
                0, 0, 1, 0,
                array[4], array[5], 0, 1
            ];
        }
        result.matrix = array;
        return result;
    };
    TransformMatrix.fromStyle = function(style, result) {
        if (!style) {
            return result || new TransformMatrix();
        }
        var transform = style.transform || style.webkitTransform || style.mozTransform;
        return TransformMatrix.fromArray(transform.replace(/^matrix(3d)?\((.*)\)$/, '$2').split(/, /), result);
    };

    if (!isTouchDevice || $.parallax.enableTouchDevices) {
        $(function() {

            $("[data-parallax]").parallax();

            

        });
    }

})(jQuery);

// isArray shim
if (!Array.isArray) {
    Array.isArray = function(arg) {
        return Object.prototype.toString.call(arg) === '[object Array]';
    };
}

// console.error shim
if (!console["error"]) {
    console.error = function(message) {
        window.alert(message);
    };
}