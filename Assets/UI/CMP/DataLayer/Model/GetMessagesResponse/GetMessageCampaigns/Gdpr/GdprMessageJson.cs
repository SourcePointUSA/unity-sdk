using System.Text.Json.Serialization;

public class GdprMessageJson
{
    [JsonInclude] public string type;
    [JsonInclude] public string name;
    [JsonInclude] public GdprMessageJsonSettings settings;
    // [JsonInclude] public List<...> children; //TODO?
    /*
                   "children":[
                  {
                     "type":"Text",
                     "name":"Text",
                     "settings":{
                        "languages":{
                           "EN":{
                              "text":"<p>TCFv2 Message Title for Language English</p>"
                           }
                        },
                        "text":"<p>TCFv2 Message Title for Language English</p>"
                     },
                     "children":[
                        
                     ]
                  },
                  {
                     "type":"Text",
                     "name":"Text",
                     "settings":{
                        "languages":{
                           "EN":{
                              "text":"<p>TCFv2 Message Title for Language English</p>"
                           }
                        },
                        "text":"<p>TCFv2 Message Title for Language English</p>"
                     },
                     "children":[
                        
                     ]
                  },
                  {
                     "type":"Row",
                     "name":"Row",
                     "settings":{
                        
                     },
                     "children":[
                        {
                           "type":"Button",
                           "name":"Button",
                           "settings":{
                              "languages":{
                                 "EN":{
                                    "text":"Accept All"
                                 }
                              },
                              "text":"Accept All",
                              "choice_option":{
                                 "type":11,
                                 "data":{
                                    "button_text":"1627028820847",
                                    "consent_origin":"https://ccpa-service.sp-prod.net",
                                    "consent_language":"EN"
                                 }
                              }
                           },
                           "children":[
                              
                           ]
                        },
                        {
                           "type":"Button",
                           "name":"Button",
                           "settings":{
                              "languages":{
                                 "EN":{
                                    "text":"Reject All"
                                 }
                              },
                              "text":"Reject All",
                              "choice_option":{
                                 "type":13,
                                 "data":{
                                    "button_text":"1627028828941",
                                    "consent_origin":"https://ccpa-service.sp-prod.net",
                                    "consent_language":"EN"
                                 }
                              }
                           },
                           "children":[
                              
                           ]
                        },
                        {
                           "type":"Button",
                           "name":"Button",
                           "settings":{
                              "languages":{
                                 "EN":{
                                    "text":"Manage Preferences"
                                 }
                              },
                              "text":"Manage Preferences",
                              "choice_option":{
                                 "type":12,
                                 "data":{
                                    "button_text":"1627028842721",
                                    "privacy_manager_iframe_url":"https://notice.sp-prod.net/privacy-manager/index.html?message_id=528826",
                                    "consent_origin":"https://ccpa-service.sp-prod.net"
                                 }
                              }
                           },
                           "children":[
                              
                           ]
                        }
                     ]
                  }
               ],
    */
}