<?php

define("isDebugMode", true);

if(isDebugMode)
{
	error_reporting(E_ALL ^ E_NOTICE);
}
else
{
	error_reporting(0);
}

$apiConfig = array(
    'DB_DRIVER' => 'mysql',
    'DB_HOST' => 'localhost',
    'DB_NAME' => 'mysql',
    'DB_USERNAME' => 'root',
    'DB_PASSWORD' => 'phpwind.net',
);

$ApiKey = 'dVoBD8W2WTXrnwCq';

$BaiduAK='bXXkWOIXEeEGAOqI9PrDsTn2';

$QQ_MAP_KEY='VD6BZ-K7ZHQ-QHF5P-G7D5G-JSMI5-Q7FLN';

$ApiVersions = array('v1');

$ApiInterfaces = array(
    'User',  //Login Register
    'Schedule', //AddSchedule UpdateSchedule DoneSchedule DeleteSchedule
);

//no access_Token and eid need
$ApiUnauthorizedActions = array(
    'User/Register/v1',
    'User/Login/v1',
    'User/CheckUserExist/v1',
    'User/GetSalt/v1'
);
function GetRandStr($len) 
{ 
    $chars = array( 
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k",  
        "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",  
        "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G",  
        "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R",  
        "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2",  
        "3", "4", "5", "6", "7", "8", "9" 
    ); 
    $charsLen = count($chars) - 1; 
    shuffle($chars);   
    $output = ""; 
    for ($i=0; $i<$len; $i++) 
    { 
        $output .= $chars[mt_rand(0, $charsLen)]; 
    }  
    return $output;  
} 



?>