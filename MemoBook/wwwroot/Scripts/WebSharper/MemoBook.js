(function(Global)
{
 "use strict";
 var MemoBook,Client,WebSharper,UI,Var$1,Concurrency,Remoting,AjaxRemotingProvider,List,Date,Doc,View,Strings;
 MemoBook=Global.MemoBook=Global.MemoBook||{};
 Client=MemoBook.Client=MemoBook.Client||{};
 WebSharper=Global.WebSharper;
 UI=WebSharper&&WebSharper.UI;
 Var$1=UI&&UI.Var$1;
 Concurrency=WebSharper&&WebSharper.Concurrency;
 Remoting=WebSharper&&WebSharper.Remoting;
 AjaxRemotingProvider=Remoting&&Remoting.AjaxRemotingProvider;
 List=WebSharper&&WebSharper.List;
 Date=Global.Date;
 Doc=UI&&UI.Doc;
 View=UI&&UI.View;
 Strings=WebSharper&&WebSharper.Strings;
 Client.deleteRegistry=function(id)
 {
  var res,_;
  res=Var$1.Create$1({
   Message:"",
   Rows:0
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.deleteRegistry:-892624886",[id]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.deleteUser=function(id)
 {
  var res,_;
  res=Var$1.Create$1({
   Message:"",
   Rows:0
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.deleteUser:-892624886",[id]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.updateRegistry=function(registry,id)
 {
  var res,_;
  res=Var$1.Create$1({
   Message:"",
   Rows:0
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.updateRegistry:819007393",[registry,id]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.updateUser=function(user,id)
 {
  var res,_;
  res=Var$1.Create$1({
   Message:"",
   Rows:0
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.updateUser:-1189104515",[user,id]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.createRegistry=function(registry)
 {
  var res,_;
  res=Var$1.Create$1({
   Message:"",
   Id:0
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.createRegistry:1213934030",[registry]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.createUser=function(user)
 {
  var res,_;
  res=Var$1.Create$1({
   Message:"",
   Id:0
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.createUser:-2059669848",[user]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.getRegistryParams=function(date,ownerid,text)
 {
  var res,_;
  res=Var$1.Create$1(List.T.Empty);
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.getRegistryParams:1634771532",[date,ownerid,text]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.getUserParams=function(name,password)
 {
  var res,_;
  res=Var$1.Create$1(List.T.Empty);
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.getUserParams:252065609",[name,password]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.getRegistry=function(id)
 {
  var res,_;
  res=Var$1.Create$1({
   id:0,
   date:Date.now(),
   ownerid:0,
   text:""
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.getRegistry:-1552829089",[id]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.getUser=function(id)
 {
  var res,_;
  res=Var$1.Create$1({
   id:0,
   name:"",
   password:""
  });
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.getUser:1735300025",[id]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.getRegistries=function()
 {
  var res,_;
  res=Var$1.Create$1(List.T.Empty);
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.getRegistries:915465954",[]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.getUsers=function()
 {
  var res,_;
  res=Var$1.Create$1(List.T.Empty);
  Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("MemoBook:MemoBook.Server.getUsers:1922418116",[]),function(a)
   {
    res.Set(a);
    return Concurrency.Zero();
   });
  })),null);
  return res.Get();
 };
 Client.About=function()
 {
  var num;
  num=Var$1.Create$1("");
  return Doc.Element("div",[],[Doc.Input([],num),Doc.Element("p",[],[Doc.TextView(View.Map(function($1)
  {
   return Strings.ToCharArray($1).slice().reverse().join("");
  },num.get_View()))])]);
 };
}(self));
