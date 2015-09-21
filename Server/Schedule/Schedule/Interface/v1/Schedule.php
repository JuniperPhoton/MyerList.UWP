<?php

do
{

	$action=$_GET['action'];
	switch ($action) {

		case 'AddSchedule':

			$sid=$_GET['sid'];
			$time=$_POST['time'];
			$content=$_POST['content'];
			$isdone=$_POST['isdone'];  // 0 for undone, 1 for done
			$cate=$_POST['cate'];

			if($sid=='' || $time=='' || $content=='' || $isdone=='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_LACK_PARAM;
				$ApiResult['error_message']='lack sid or time or content or isdone';
				break;
			}

			$queryFind=$pdo->prepare('SELECT * FROM user WHERE sid=:sid');
			$queryFind->bindParam(':sid',$sid,PDO::PARAM_INT);			
			$resultFind=$queryFind->execute();
			if($resultFind)
			{
				$sche=$queryFind->fetch();
				if(!$sche)
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=API_ERROR_USER_NOTEXIST;
					$ApiResult['error_message']='API_ERROR_USER_NOTEXIST';
					break;
				}
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}

			$queryInsert=$pdo->prepare('INSERT INTO schedule(sid,time,content,isdone) VALUES (:sid,:time,:content,:isdone)');
			$queryInsert->bindParam(':sid',$sid,PDO::PARAM_INT);
			$queryInsert->bindParam(':time',$time,PDO::PARAM_STR);
			$queryInsert->bindParam(':content',$content,PDO::PARAM_STR);
			$queryInsert->bindParam(':isdone',$isdone,PDO::PARAM_INT);
			$resultInsert=$queryInsert->execute();
			if($resultInsert)
			{
				$newid=$pdo->lastInsertId();
				$ApiResult['isSuccessed']=true;
				$ApiResult['error_code']=0;
				$ApiResult['error_message']='';
				$ApiResult['ScheduleInfo']=array('id'=>$newid,'sid'=>$sid,'time'=>$time,'content'=>$content,'isdone'=>$isdone);
				break;
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}
			break;

		case 'UpdateContent':

			$id=$_POST['id'];
			$content=$_POST['content'];
			$cate=$_POST['cate'];
			
			if($cate=='') $cate=0;  
			
			if($id=='' || $content=='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_LACK_PARAM;
				$ApiResult['error_message']='lack id or content';
				break;
			}

			$queryFind=$pdo->prepare('SELECT * FROM schedule WHERE id=:id');
			$queryFind->bindParam(':id',$id,PDO::PARAM_INT);			
			$resultFind=$queryFind->execute();
			if($resultFind)
			{
				$sche=$queryFind->fetch();
				if(!$sche)
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=API_ERROR_SCHEDULE_NOT_EXIST;
					$ApiResult['error_message']='API_ERROR_SCHEDULE_NOT_EXIST';
					break;
				}
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}


			$queryUpdate=$pdo->prepare('UPDATE schedule SET content=:content AND cate=:cate WHERE id=:id');
			$queryUpdate->bindParam(':id',$id,PDO::PARAM_INT);
			$queryUpdate->bindParam(':content',$content,PDO::PARAM_STR);
			$queryUpdate->bindParam(':cate',$cate,PDO::PARAM_INT);
			
			$resultUpdate=$queryUpdate->execute();
			if($resultUpdate)
			{
				$ApiResult['isSuccessed']=true;
				$ApiResult['error_code']=0;
				$ApiResult['error_message']='';
				break;
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}
			break;
		
		case 'FinishSchedule':

			$id=$_POST['id'];
			$isdone=$_POST['isdone'];

			if($id=='' || $isdone=='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_LACK_PARAM;
				$ApiResult['error_message']='lack id or isdone';
				break;
			}

			$queryFind=$pdo->prepare('SELECT * FROM schedule WHERE id=:id');
			$queryFind->bindParam(':id',$id,PDO::PARAM_INT);			
			$resultFind=$queryFind->execute();
			if($resultFind)
			{
				$sche=$queryFind->fetch();
				if(!$sche)
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=API_ERROR_SCHEDULE_NOT_EXIST;
					$ApiResult['error_message']='API_ERROR_SCHEDULE_NOT_EXIST';
					break;
				}
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}


			$queryUpdate=$pdo->prepare('UPDATE schedule SET isdone=:isdone WHERE id=:id');
			$queryUpdate->bindParam(':id',$id,PDO::PARAM_INT);
			$queryUpdate->bindParam(':isdone',$isdone,PDO::PARAM_INT);
			
			$resultUpdate=$queryUpdate->execute();
			if($resultUpdate)
			{
				$ApiResult['isSuccessed']=true;
				$ApiResult['error_code']=0;
				$ApiResult['error_message']='';
				break;
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}
			
		case 'DeleteSchedule':

			$id=$_POST['id'];
			
			if($id=='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_LACK_PARAM;
				$ApiResult['error_message']='lack id';
				break;
			}

			$queryFind=$pdo->prepare('SELECT * FROM schedule WHERE id=:id');
			$queryFind->bindParam(':id',$id,PDO::PARAM_INT);			
			$resultFind=$queryFind->execute();
			if($resultFind)
			{
				$sche=$queryFind->fetch();
				if(!$sche)
				{
					$ApiResult['isSuccessed']=false;
					$ApiResult['error_code']=API_ERROR_SCHEDULE_NOT_EXIST;
					$ApiResult['error_message']='API_ERROR_SCHEDULE_NOT_EXIST';
					break;
				}
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}


			$queryDelete=$pdo->prepare('DELETE FROM schedule WHERE id=:id');
			$queryDelete->bindParam(':id',$id,PDO::PARAM_INT);
			
			$resultDelete=$queryDelete->execute();
			if($resultDelete)
			{
				$ApiResult['isSuccessed']=true;
				$ApiResult['error_code']=0;
				$ApiResult['error_message']='';
				break;
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}

		case 'GetMySchedules':

			$sid=$_POST['sid'];

			if($sid=='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_LACK_PARAM;
				$ApiResult['error_message']='API_ERROR_LACK_PARAM';
				break;
			}
			
			$queryGet=$pdo->prepare('SELECT * FROM schedule WHERE sid=:sid ORDER BY id');
			$queryGet->bindParam(':sid',$sid,PDO::PARAM_INT);
			$resultGet=$queryGet->execute();
			if($resultGet)
			{
				$ApiResult['isSuccessed']=true;
				$ApiResult['error_code']=0;
				$ApiResult['error_message']='';
				$ApiResult['ScheduleInfo']=$queryGet->fetchAll();
				break;
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}
			break;



		case 'GetMyOrder':

			$sid=$_POST['sid'];

			if($sid=='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_LACK_PARAM;
				$ApiResult['error_message']='API_ERROR_LACK_PARAM';
				break;
			}
			
			$queryGet=$pdo->prepare('SELECT list_order FROM user WHERE sid=:sid');
			$queryGet->bindParam(':sid',$sid,PDO::PARAM_INT);
			$resultGet=$queryGet->execute();
			if($resultGet)
			{
				$ApiResult['isSuccessed']=true;
				$ApiResult['error_code']=0;
				$ApiResult['error_message']='';
				$ApiResult['OrderList']=$queryGet->fetchAll();
				break;
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				$ApiResult['error_message']=$queryInsert->errorInfo();
				break;
			}
			break;

		case 'SetMyOrder':

			$sid=$_POST['sid'];
			$order=$_POST['order'];

			if($sid=='' || $order=='')
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_LACK_PARAM;
				$ApiResult['error_message']='API_ERROR_LACK_PARAM';
				break;
			}
			
			$querySet=$pdo->prepare('UPDATE user SET list_order=:order WHERE sid=:sid');
			$querySet->bindParam(':sid',$sid,PDO::PARAM_INT);
			$querySet->bindParam(':order',$order,PDO::PARAM_STR);
			$resultSet=$querySet->execute();
			if($resultSet)
			{
				$ApiResult['isSuccessed']=true;
				$ApiResult['error_code']=0;
				$ApiResult['error_message']='';
				break;
			}
			else
			{
				$ApiResult['isSuccessed']=false;
				$ApiResult['error_code']=API_ERROR_DATABASE_ERROR;
				
				break;
			}
			break;


		default:
			# code...
			break;
	}

}while(0);


?>