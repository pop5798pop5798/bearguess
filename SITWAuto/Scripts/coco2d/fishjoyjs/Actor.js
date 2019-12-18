(function(){
	var Actor=window.Actor=Class.extend({
		init:function(){
			g.actors.push(this);
		},
		render:function(){
			throw Error("遊戲必須炫染");
		},
		update:function(){
			
		}
	});
})();