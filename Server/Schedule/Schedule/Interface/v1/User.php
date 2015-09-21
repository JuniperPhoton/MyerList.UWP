<?php

do
{

	$action=$_GET['action'];
	switch ($action) {
		case 'CheckUserExist':
			$email=$_POST['email'];
			$queryFind=$pdo->prepare('SELECT * FROM User WHERE email=:email');
			$queryFind->bindParam(':email',$email,PDO::PARAM_STR);
			$result=$queryFind->execute();
			if($result)
			{
				$user=$queryFind->fetch();
				if($user)
				{
					$ApiResult['isSuccessed']=true;
					$ApiResult['error_code']=0;
					$ApiResult['error_message']='';
					$ApiResult['isExist']=true;
					break;
				}
				else
				{
					$ApiResult['isSuccessed']=true;
					$ApiResult['error_code']=0;
					$ApiResult['error_message']='';
					$ApiResult['isExist']=false;
					break;
				}
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']='database error';
				break;
			}
			break;

		case 'GetSalt':
			$email=$_POST['email'];

			$queryFind=$pdo->prepare('SELECT salt from User WHERE email=:email');
			$queryFind->bindParam(':email',$email,PDO::PARAM_STR);
			$result=$queryFind->execute();
			if($result)
			{
				$salt=$queryFind->fetch();
				if($salt)
				{
					$ApiResult['isSuccessed']=true;
					$ApiResult['error_code']=0;
					$ApiResult['error_message']='';
					$ApiResult['Salt']=$salt['salt'];
					break;
				}
				else
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=0;
					$ApiResult['error_message']='NO SALT';
					//$ApiResult['Salt']=$salt;
					break;
				}
			}
			else 
			{

			}
			break;
		case 'Register':
			
			$email=$_POST['email'];
			$password=$_POST['password']; //the password is after md5 from client
			//echo $password;
			if($email=='' || $password =='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_PARM_LACK;
				$ApiResult['error_message']='lack email or password ';
				break;
			}



			$queryFind=$pdo->prepare('SELECT * FROM User WHERE email=:email');
			$queryFind->bindParam(':email',$email,PDO::PARAM_STR);
			$resultFind=$queryFind->execute();
			if($resultFind)
			{
				//print_r($queryFind->fetch());
				if($queryFind->fetch())
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=API_ERROR_USER_ALEADY_EXIST;
					$ApiResult['error_message']='the email has already used';
					break;
				}
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']='database error';
				break;
			}
			$salt=GetRandStr(10);
			$psSalt=md5($password.$salt);

			$queryInsert=$pdo->prepare('INSERT INTO User(email,password,salt) VALUES (:email,:password,:salt)');
			$queryInsert->bindParam(':email',$email,PDO::PARAM_STR);
			$queryInsert->bindParam(':password',$psSalt,PDO::PARAM_STR);
			$queryInsert->bindParam(':salt',$salt,PDO::PARAM_STR);
			
			$resultInsert=$queryInsert->execute();
			if($resultInsert)
			{
				$sid=$pdo->lastInsertId();
				$accessToken=md5($sid);

				$queryInsert=$pdo->prepare('INSERT INTO AccessToken(sid,access_token) VALUES (:sid,:access_token)');
				$queryInsert->bindParam(':sid',$sid,PDO::PARAM_INT);
				$queryInsert->bindParam(':access_token',$accessToken,PDO::PARAM_STR);
				$resultInsert=$queryInsert->execute();
				if($resultInsert)
				{
					$ApiResult['isSuccessed']=true;
					$ApiResult['error_code']=0;
					$ApiResult['error_message']='';
					$ApiResult['UserInfo']=array('email'=>$email,'name'=>$name,'sid'=>$sid,'access_token'=>$accessToken,'Salt'=>$salt);
					break;
				}
				else
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
					$ApiResult['error_message']='database error';
					break;
				}
				
			}		
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']='database error';
				break;
			}	
		
		case 'Login':
			
			$email=$_POST['email'];
			$password=$_POST['password'];

			if($email=='' || $password =='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_PARM_LACK;
				$ApiResult['error_message']='lack email or password or name';
				break;
			}

			$queryFind=$pdo->prepare('SELECT * FROM user WHERE email=:email && password=:password');
			$queryFind->bindParam(':email',$email,PDO::PARAM_STR);
			$queryFind->bindParam(':password',$password,PDO::PARAM_STR);
			$result=$queryFind->execute();
			if($result)
			{
				$user=$queryFind->fetch();
				if($user)
				{

					$querySelect=$pdo->prepare('SELECT * FROM accesstoken WHERE sid=:sid');
					$querySelect->bindParam(':sid',$user['sid'],PDO::PARAM_INT);
					$resultSelect=$querySelect->execute();
					if($resultSelect)
					{
						$accessToken=$querySelect->fetch();
						if($accessToken)
						{
							$ApiResult['isSuccessed']=true;
							$ApiResult['error_code']=0;
							$ApiResult['error_message']='';
							$ApiResult['UserInfo']=array('sid'=>$user['sid'],'access_token'=>$accessToken['access_token']);
							break;
						}
					}
					else
					{
						$ApiResult['isSuccessed']=false;
						$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
						$ApiResult['error_message']='database error';
						break;
					}
				}
				else
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=API_ERROR_USER_NOT_EXIST;
					$ApiResult['error_message']='the user does not exists';
					break;
				}
			}
			else
			{

			}
			
		default:
			# code...
			break;
	}

}while(0);


?>