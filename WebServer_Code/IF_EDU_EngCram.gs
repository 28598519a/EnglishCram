/* 注意: 不再提供1.0以下版本更新 */
var SpreadSheet = SpreadsheetApp.openById("1xy0WYAAUCj7Xdd9vh0PulLvWsYBSt7qm6DLGEjTFBTk");
var Sheet = SpreadSheet.getSheetByName("s01");

//C#端傳值方式是POST,所以這裡必須是doPost
function doPost(e) {
  var params=e.parameter;
  var RtnVersion = {
    "Rtn_EngCram":{}
  };
  var d1=params.ClientID;//User IDentity
  var d2=params.ClientVersion;//User Version

  /* enum Col */
  var Col;
  (function (Col) {
    Col[Col["GetVersion"] = 1] = "GetVersion";
    Col[Col["GetReleaseUrl"] = 2] = "GetReleaseUrl";
    Col[Col["GetReleaseDate"] = 3] = "GetReleaseDate";
    Col[Col["GetMainName"] = 4] = "GetMainName";
    Col[Col["TrafficAnalytics"] = 6] = "TrafficAnalytics";
  })(Col || (Col = {}));

  if(typeof(d2) !== "undefined"){
    RtnVersion.Rtn_EngCram = addValueInObject(RtnVersion.Rtn_EngCram,Sheet.getRange(1,Col.GetVersion).getValue(), Sheet.getRange(2,Col.GetVersion).getValue());
    RtnVersion.Rtn_EngCram = addValueInObject(RtnVersion.Rtn_EngCram,Sheet.getRange(1,Col.GetReleaseUrl).getValue(), Sheet.getRange(2,Col.GetReleaseUrl).getValue());
    RtnVersion.Rtn_EngCram = addValueInObject(RtnVersion.Rtn_EngCram,Sheet.getRange(1,Col.GetReleaseDate).getValue(), Sheet.getRange(2,Col.GetReleaseDate).getValue());
    RtnVersion.Rtn_EngCram = addValueInObject(RtnVersion.Rtn_EngCram,Sheet.getRange(1,Col.GetMainName).getValue(), Sheet.getRange(2,Col.GetMainName).getValue());
    //流量分析 (如果客戶端版本 > 伺服端，代表開發人員使用，不紀錄)
	if(d2 < Sheet.getRange(2,Col.GetVersion).getValue()){
      Sheet.getRange(2,Col.TrafficAnalytics).setValue(Sheet.getRange(2,Col.TrafficAnalytics).getValue() + 1);
	}
  }
  return ContentService.createTextOutput(JSON.stringify(RtnVersion));
}

function Debug() {
  var e = {
    "parameter":{
      "ClientID": "a",
      "ClientVersion": "1.6"
    }
  }
  var result = doPost(e);
  Logger.log("%s", result.getContent());
}

//仍然保留Url傳值的接收方式
function doGet(e){
  return doPost(e);
}

function addValueInObject(object, key, value) {
    var res = {};
    var textObject = JSON.stringify(object);
    if (textObject === '{}') {
        res = JSON.parse('{"' + key + '":"' + value + '"}');
    } else {
        res = JSON.parse('{' + textObject.substring(1, textObject.length - 1) + ',"' + key + '":"' + value + '"}');
    }
    return res;
}