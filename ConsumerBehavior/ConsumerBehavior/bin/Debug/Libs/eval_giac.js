giaceval = Module.cwrap('caseval', 'string', ['string']);
		
	var UI = {
	usemathjax:true
	latexeval : function(input){
    var tmp=input;
    if (tmp.length>5 && tmp.substr(0,5)=='gl3d_') return tmp;
    if (tmp.length>5 && tmp.substr(1,4)=='<svg') return tmp.substr(1,tmp.length-2);
    if (tmp.length>5 && tmp.substr(0,4)=='<svg') return tmp;
     if (UI.usemathjax){
	 
       var inp =giaceval('latex(quote('+input+'))');
       var result=giaceval('latex(quote('+giaceval(input)+'))');
       var dollar=String.fromCharCode(36);
	
     result = dollar+dollar+inp.substr(1,inp.length-2)+dollar+dollar;
       return result;
     }
	var expr = giaceval('mathml(quote('+input+ ' = ' + giaceval(input) +', 1))');   
     expr=expr.substr(1,expr.length-2); 
    return expr;   
  }
  };

function solve_eq(eq, var)
{
	return UI.latexeval("solve(eq=0, var)");
}