<?php
header("Content-Type: application/json; charset=UTF-8");
define("DB_SERVER", "localhost");
define("DB_USER", "uaqkgjeoxf1d5");
define("DB_PASSWORD", "d?d223(7_622");
define("DB_NAME", "dbrabzez6wszm1");

define("ACCESS_TOKEN", "9D2C1750509C6963BD4B108AA8DB2B54BEF10E5F");
//print_r($_POST);
//echo "=============== ". ACCESS_TOKEN . " =======================\n";
//print_r($_REQUEST);

$output = array();

if((!isset($_POST['ACCESS_TOKEN'])) or (empty($_POST['ACCESS_TOKEN'])) or (strcmp($_POST["ACCESS_TOKEN"], ACCESS_TOKEN) !== 0)) {
   $output["code"] = 1;
   $output["message"] = "Access to resource denied!";
   echo json_encode($output, JSON_UNESCAPED_UNICODE);
   die();
}
$action = "";
if((!isset($_POST['action'])) or (empty($_POST['action']))) {
   $output["code"] = 2;
   $output["message"] = "Action is not set!";
   echo json_encode($output, JSON_UNESCAPED_UNICODE);
   die();
}else{
   $action = strtolower($_POST['action']);
}
$table = "";
if((!isset($_POST['table'])) or (empty($_POST['table']))) {
   $output["code"] = 3;
   $output["message"] = "Table is not set!";
   echo json_encode($output, JSON_UNESCAPED_UNICODE);
   die();
}else{
   $table = DB_NAME . "." . strtolower($_POST['table']);
}

$sql="";

$output["action"] = $action;
$output["sql"] = "Empty"; 

if( $action == "select"){
   $sql = "SELECT * FROM ". $table . "" ;
}elseif($action == "update"){
    if((!isset($_POST['name'])) or (empty($_POST['name']))) {
      $output["code"] = 5;
      $output["message"] = "Name field is not set!";
      echo json_encode($output, JSON_UNESCAPED_UNICODE);
      die();
   }elseif((!isset($_POST['data'])) or (empty($_POST['data']))) {
      $output["code"] = 10;
      $output["message"] = "Data is not set!";
      echo json_encode($output, JSON_UNESCAPED_UNICODE);
      die();
      }
   else{
      /*Needs more work*/
      $sql = "UPDATE " . $table ." SET " .
                     "`timestamp` = CURRENT_TIMESTAMP, ".
                     "`displayname`='". $_POST['displayname'] .
                     "', `points`=".$_POST['points'] .
                     ", `power`=".$_POST['power'] .
                     ", `level`=".$_POST['level'] .
                     ", `avatar`='".$_POST['avatar'] .
                     "', `characters`='".$_POST['characters'] .
                     "', `data`='" . $_POST['data'] .
                     "'  WHERE `username` = '" . $_POST['name']. "'";
      //$sql = stripslashes($sql);
   }
}elseif($action == "insert"){
   //inserting new user; 1st check parameters
   if((!isset($_POST['name'])) or (empty($_POST['name']))) {
      $output["code"] = 5;
      $output["message"] = "Name field is not set!";
      echo json_encode($output, JSON_UNESCAPED_UNICODE);
      die();
   }elseif((!isset($_POST['email'])) or (empty($_POST['email']))) {
      $output["code"] = 6;
      $output["message"] = "Email field is not set!";
      echo json_encode($output, JSON_UNESCAPED_UNICODE);
      die();

   }elseif((!isset($_POST['password'])) or (empty($_POST['password']))) {
      $output["code"] = 7;
      $output["message"] = "Password field is not set!";
      echo json_encode($output, JSON_UNESCAPED_UNICODE);
      die();

   }else{
      $sql = "INSERT INTO {$table} (`username`,`email`,`password`,`data`) VALUES('".$_POST['name']."', '" .$_POST['email']."', '". md5($_POST['password'])."', '" .$_POST['data']."')";

   }
}elseif($action == "checkuser"){

   if((!isset($_POST['name'])) or (empty($_POST['name']))) {
      $output["code"] = 8;
      $output["message"] = "Name to check is not set!";
      echo json_encode($output, JSON_UNESCAPED_UNICODE);
      die();
   }else{
      $sql = "SELECT * FROM {$table} WHERE {$table}.username = '".$_POST['name']."'"; 
      
   }

}elseif($action == "checkemail"){

   if((!isset($_POST['email'])) or (empty($_POST['email']))) {
      $output["code"] = 9;
      $output["message"] = "Email to check is not set!";
      echo json_encode($output, JSON_UNESCAPED_UNICODE);
      die();
   }else{
      $sql = "SELECT * FROM {$table} WHERE {$table}.email = '".$_POST['email']."'"; 
      
   }

}else{
   $output["code"] = 4;
   $output["message"] = "Incorrect action specified!";
   echo json_encode($output, JSON_UNESCAPED_UNICODE);
   die();
}
$output["sql"] = $sql;

$mysqli = new mysqli(DB_SERVER, DB_USER, DB_PASSWORD, DB_NAME);

if ($mysqli->connect_error) {
   error_log('Connection error: ' . $mysqli->connect_error);   
   $output["code"] = 404;
   $output["message"] = 'Connection error: ' . $mysqli->connect_error;
   echo json_encode($output, JSON_UNESCAPED_UNICODE);
   die();
}else{
  //echo 'Connected';
}

$mysqli->select_db(DB_NAME) or die( "Unable to select database");

$mysqli->set_charset("utf8");

$result = $mysqli->query($sql);
$results = [];

while($row = mysqli_fetch_assoc($result)) {
   $results[] = $row;
} 

$output["code"] = 200;
$output["message"] = "Success";

if($action == "checkuser" || $action == "checkemail"){
   $output["user"] =  $results[0]["username"];
   $output["pass"] =  $results[0]["password"];
   $output["displayname"] =  $results[0]["displayname"];
   $output["email"] =  $results[0]["email"];
   $output["points"] =  $results[0]["points"];
   $output["avatar"] =  $results[0]["avatar"];
   $output["data"] = $results[0]["data"];  
}

//$output["sql"] =""; 


//$output["data"] = $results;

echo json_encode($output, JSON_UNESCAPED_UNICODE);

$mysqli->close();

?>