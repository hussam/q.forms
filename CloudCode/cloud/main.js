var Twilio = require('twilio')('AC698130a23f7711c39d6c0a72ccea4d52', '7f0ab4f853bc1bd8b03f4d3a9a4a45ef');
var MD5 = require('cloud/md5.js');

Parse.Cloud.afterSave("Message", function(request) {
   var sender = request.object.get("sender");
   var message = request.object.get("text");
   var discussionId = request.object.get("discussion").id;

   var pushRecipients = new Parse.Query(Parse.Installation);
   pushRecipients.equalTo("channels", discussionId);
   pushRecipients.notEqualTo("owner", sender);

   var discussionQuery = new Parse.Query("Discussion");
   discussionQuery.include("activity");

   discussionQuery.get(discussionId).then(
      function(discussion) {
         var activity = discussion.get("activity").get("name");
         sender.fetch().then(
            function(sender) {
               var name = sender.get("name");
               Parse.Push.send({
                  where: pushRecipients,
                  data: {
                     alert: name + " @ \"" + activity + "\": " + message,
                     sound: "default"
                  }
               });
            });
      });
});


Parse.Cloud.afterSave("Discussion", function(request) {
   Parse.Cloud.useMasterKey();
   var discussionId = request.object.id;
   var query = new Parse.Query(Parse.Installation);
   query.containedIn("owner", request.object.get("participants"));
   query.find({
      success: function(installations) {
         for (var i=0; i < installations.length; i++) {
            installations[i].addUnique("channels", discussionId);
            installations[i].save();
         }
      },
      error: function(error) {
         console.log("Error: " + error.code + " " + error.message);
      }
   });
});

Parse.Cloud.beforeSave(Parse.User, function(request, response) {
   var username = request.object.get("username");
   request.object.set("hash", MD5.b64_md5(username));
   response.success();
});

Parse.Cloud.define("ProcessContacts", function(request, response) {
   var Contact = Parse.Object.extend("Contact");
   var currentUser = Parse.User.current();

   var contacts = [];
   var hashes = [];
   for (var i=0; i < request.params.contacts.length; i++) {
      var hash = request.params.contacts[i].base64;
      hashes.push(hash);

      var contact = new Contact();
      contact.set("owner", currentUser);
      contact.set("phoneNumber", hash);
      contacts.push(contact);
   }

   Parse.Object.saveAll(contacts, {
      success: function(contacts) {
         // get all user accounts that correspond to contacts given by caller
         var innerQuery = new Parse.Query(Parse.User);
         innerQuery.containedIn("hash", hashes);

         // narrow down the previous list of user accounts to those who have
         // the current user as a contact
         var query = new Parse.Query(Contact);
         query.matchesQuery("owner", innerQuery);
         query.equalTo("phoneNumber", currentUser.get("hash"));
         query.include("owner");

         query.find({
            success: function(friends) {
               var usersToSave = [];
               for (var i=0; i < friends.length; i++) {
                  var otherUser = friends[i].get("owner");
                  if (otherUser.id != currentUser.id) {
                     currentUser.addUnique("friends", otherUser);
                     otherUser.addUnique("friends", currentUser);
                     usersToSave.push(otherUser);
                  }
               }
               if (usersToSave.length > 0) {
                  usersToSave.push(currentUser);
                  Parse.Object.saveAll(usersToSave, {
                     useMasterKey : true
                  }).then( function() { response.success(true); } );
               } else {
                  response.success(true);
               }
            },
            error: function(error, friends) {
               console.log("Error " + error.code + ": " + error.message);
               response.error("Failed in finding friends");
            }
         });
      },
      error: function(error, contacts) {
         console.log("Error " + error.code + ": " + error.message);
         response.error("Failed in processing contacts");
      }
   });
});

Parse.Cloud.define("RegisterAndSendVerificationCode", function(request, response) {
   Parse.Cloud.useMasterKey();
   var code = (1000 + Math.floor(Math.random() * 8999)).toString();
   var query = new Parse.Query(Parse.User);
   query.equalTo("username", request.params.number);
   query.find({
      success: function(users) {
         Twilio.sendSms({
            From: "+16073337775",
            To: request.params.number,
            Body: "Your wim verification code is " + code + "."
         }, function(err, responseData) {
            if (err) {
               response.error(err);
            } else {
               if (users.length > 0) {
                  var u = users[0];
                  u.set("password", code);
                  u.save(null, {
                     success: function(u) {
                        response.success(true);
                     },
                     error: function(u, error) {
                        console.log("Error " + error.code + " " + error.message);
                        response.error("Failed to modify user account.");
                     }
                  });
               } else {
                  var user = new Parse.User();
                  user.signUp({
                     username: request.params.number,
                     password: code
                  }, {
                     success: function(user) {
                        response.success(true);
                     },
                     error: function(user, error) {
                        console.log("Error " + error.code + " " + error.message);
                        response.error("Failed to register user.");
                     }
                  });
               }
            }
         });
      }
   });
});


Parse.Cloud.define("getRecommendation", function(request, response) {
   var Recipe = Parse.Object.extend("Recipe");
   var query = new Parse.Query(Recipe);
   query.get("lGV4cXHbAR").then(function(recipe) {
      response.success({
         "profile" : {
            "Chicken" : 0.73124,
            "Pesto" : 0.499,
            "Vegetable" : 0.303
            },
         "recipe" : recipe
      });
   }, function(error) {
      response.error(error);
   });
   var Crawler = Parse.Object.extend("Crawler");

});


/******************
 * Yummly Crawler *
 ******************/

const YUMMLY_OBJID = "Uawb2leC2K";

Parse.Cloud.define("setYummlyCrawlerSettings", function(request, response) {
   var query = new Parse.Query("Crawler");
   query.get(YUMMLY_OBJID).then(function(crawler) {
      var settings = crawler.get("data");

      var crawledCuisines = [
         "American",
         "Italian",
         "Mexican",
         "Indian",
         "Thai",
         "Chinese",
         "Japanese",
         "Asian",
         "Mediterranean"
      ];
      settings["crawledCuisines"] = crawledCuisines;
      settings["requestsPerDay"] = 250;

      if (settings["numberCrawled"] == null) {
         settings["numberCrawled"] = 0;
      }

      // save the settings back into crawler data
      crawler.set("data", settings);
      crawler.save().then(function(obj) {
         response.success(true);
      });
   });
});

Parse.Cloud.define("getYummlyCuisines", function(request, response) {
   var query = new Parse.Query("Crawler");
   query.get(YUMMLY_OBJID).then(function(crawler) {
      var settings = crawler.get("data");

      Parse.Cloud.httpRequest({
         url: settings.baseurl + "metadata/cuisine",
         headers: {
            'X-Yummly-App-ID'  : settings.appid,
            'X-Yummly-App-Key' : settings.appkey
         }
      }).then( function(httpResponse) {
         function set_metadata(_name, data) {
            var cuisines = [];
            for (var i=0; i < data.length; i++) {
               cuisines.push( {
                  name : data[i].name,
                  searchValue : data[i].searchValue
               });
            }
            settings["cuisines"] = cuisines;
            crawler.set("data", settings);
            crawler.save().then(function(obj) {
               response.success("Got " + cuisines.length + " cuisines.");
            });
         };
         eval(httpResponse.text);
      });
   });
});

Parse.Cloud.define("crawlRecipe", function(request, response) {
   var Recipe = Parse.Object.extend("Recipe");

   var baseurl  = request.params.baseParams.baseurl;
   var appid    = request.params.baseParams.appid;
   var appkey   = request.params.baseParams.appkey;
   var recipeId = request.params.recipeId;

   return Parse.Cloud.httpRequest({
      url: baseurl + 'recipe/' + recipeId,
      headers: {
       'X-Yummly-App-ID'  : appid,
       'X-Yummly-App-Key' : appkey
      }
   }).then(function(httpResponse) {
      var recipeDetails = httpResponse.data;

      // Create a DB record for the recipe
      var recipe = new Recipe();
      recipe.set("slug", recipeId);
      recipe.set("name", recipeDetails.name);
      recipe.set("cuisine", recipeDetails.attributes.cuisine);
      recipe.set("rawInfo", recipeDetails);

      // Fetch the recipe picture
      var imgUrl = recipeDetails.images[0].hostedLargeUrl;
      return Parse.Cloud.httpRequest({
         url: imgUrl
      }).then(function(httpResponse) {
         var file = new Parse.File("recipe", {
            base64: httpResponse.buffer.toString('base64', 0, httpResponse.buffer.length)
         });
         return file.save().then(function() {
            // We finally have the recipe details and the
            // image. Save the records and process next
            // recipe.
            recipe.set("picture", file);
            return recipe.save().then(function() {
               return true;
            }, function(error) {
               console.log("Oh no! --- " + error.message);
               return false;
            });
         });
      }, function(error) {
         console.log("Got an error");
         consloe.log(error.message);
      });
   }).then(function() {
      response.success("Done");
   });
});

Parse.Cloud.job("getYummlyRecipes", function(request, status) {
   var Crawler = Parse.Object.extend("Crawler");
   var query = new Parse.Query(Crawler);
   query.get(YUMMLY_OBJID).then(function(crawler) {
      // Update the crawler info in the DB
      crawler.set("isRunning", true);
      crawler.set("lastRun", new Date());
      crawler.save();

      var settings = crawler.get("data");

      // set up parameters for recipes search
      var startIndex    = parseInt(settings["numberCrawled"]);     // pagination start
      var numberToCrawl = parseInt(settings["requestsPerDay"]);    // number of results to return

      var reqQueryString = "requirePicture=true&maxResult=" + numberToCrawl + "&start=" + startIndex;

      // build a list of cuisine search-terms (English -> Yummly Metadata terms)
      var allCuisines = settings["cuisines"];
      var crawledCuisines = settings["crawledCuisines"];
      for (var i=0; i < crawledCuisines.length; i++) {
         reqQueryString += "&allowedCuisine[]=" + allCuisines[crawledCuisines[i]];
      }

      return Parse.Cloud.httpRequest({
         url: settings.baseurl + 'recipes?' + reqQueryString,
         headers: {
            'X-Yummly-App-ID'  : settings.appid,
            'X-Yummly-App-Key' : settings.appkey
         }
      }).then( function(httpResponse) {
         var baseParams = {
            "baseurl" : settings.baseurl,
            "appid"   : settings.appid,
            "appkey"  : settings.appkey
         };
         function doCrawl(recipes, counter) {
            if (recipes.length > 0) {
               if (counter % 25 == 0) {
                  status.message(counter + " recipes crawled.");
               }
               Parse.Cloud.run("crawlRecipe", {"baseParams": baseParams, "recipeId": recipes.pop().id}, {
                  success: function(result) {
                     doCrawl(recipes, counter + 1);
                  },
                  error: function(result) {
                     doCrawl(recipes, counter + 1);
                  }
               });
            } else {
               crawler.set("isRunning", false);
               settings["numberCrawled"] = startIndex + numberToCrawl;
               crawler.set("data", settings);
               crawler.save().then(function() {
                  status.success("Done");
               }, function() {
                  status.error("Error - " + error.message);
               });
            }
         }
         doCrawl(httpResponse.data.matches, 0);
      }, function(error) {
         console.log("Error searching for recipes - " + error);
      });
   });
});
