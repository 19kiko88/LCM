import { Component } from '@angular/core';
import { IResultDto } from 'src/app/shared/models/dto/response/result-dto';
import { BsEighteenService } from '../../../../core/http/bs-eighteen.service'
import { DatePipe } from '@angular/common'
import Swal from 'sweetalert2';
import {SweetAlertService} from '../../../../core/service/sweet-alert.service'


@Component({
  selector: 'app-bs-eighteen',
  templateUrl: './bs-eighteen.component.html',
  styleUrls: ['./bs-eighteen.component.scss']
})

export class BsEighteenComponent 
{  
  //@ViewChild("upload_3") upload_3?: UploadComponent;
  isLoading: boolean = false;
  loadingMsg: string = "";
  notificationS18: string = "";
  notificationB18: string = "";

  constructor(
    private _bsEighteenService: BsEighteenService,
    private swl: SweetAlertService,
    private datepipe: DatePipe
  ){  }

  reportUpload(data: IResultDto<string>)
  {
    this.loadingMsg = "資料處理中..."
    this.isLoading = true;

    if(!data.success && data.message)
    {
      Swal.fire('', data.message, 'error');
      this.isLoading = false;
      return;
    }
    else 
    {
      if (data.message == "s18") 
      {
        console.log("s18 insert start.");
        this._bsEighteenService.insertS18(data.content).subscribe({
          next: (res) => 
          {
            if (res.success) 
            {
              this.swl.showSwal('', '小18報表匯入完成.', 'info');
              this.notificationS18 = `上傳日期：${this.datepipe.transform(res.content.uploadDT, 'yyyy/MM/dd HH:mm:ss')}; ESB日期：${this.datepipe.transform(res.content.esbDTStart, 'yyyy/MM/dd')}~${this.datepipe.transform(res.content.esbDTSEnd, 'yyyy/MM/dd')}; 上傳資料筆數：${res.content.uploadDataCount}/${res.content.totalDataCount}`;
            }            
            else 
            {
              this.swl.showSwal('', `小18報表匯入異常，請聯絡CAE Team.<br\>${res.message}`, 'error');
            }
          },
          error: (err) =>{
            this.swl.showSwal('', '小18報表匯入異常，請聯絡CAE Team.', 'error');
            this.isLoading = false;
          },
          complete: () =>{
            this.isLoading = false;
          }
        })
      }
      else if (data.message == "b18") 
      {
        console.log("b18 insert start.");
        this._bsEighteenService.insertB18(data.content).subscribe({
          next: (res) => 
          {
            if (res.success) 
            {
              this.swl.showSwal('', '大18報表匯入完成.', 'info');
              this.notificationB18 = `上傳日期：${this.datepipe.transform(res.content.uploadDT, 'yyyy/MM/dd HH:mm:ss')}; Data Market日期：${this.datepipe.transform(res.content.dataMarketDTStart, 'yyyy/MM/dd')}~${this.datepipe.transform(res.content.dataMarketDTEnd, 'yyyy/MM/dd')}; 上傳資料筆數：${res.content.uploadDataCount}/${res.content.totalDataCount}`;
            }            
            else 
            {
              this.swl.showSwal('', `大18報表匯入異常，請聯絡CAE Team.<br\>${res.message}`, 'error');
            }
          },
          error: (err) =>{
            this.swl.showSwal('', '大18報表匯入異常，請聯絡CAE Team.', 'error');
            this.isLoading = false;
          },
          complete: () =>{
            this.isLoading = false;
          }
        })
      }
      else if (data.message == "vendor") 
      {
        console.log("pk export start.");
        this._bsEighteenService.exportPK(data.content).subscribe(res => {
          if (res.success)
          {
            this.swl.showSwal('', 'pk報表產出成功.', 'info');
          }
          else 
          {
            this.swl.showSwal('', `pk報表產出異常，請聯絡CAE Team.<br\>${res.message}`, 'error');
          }

          console.log("pk export end.");
          this.isLoading = false;
        })
      }
    }
  }

  //上傳元件的output loading遮罩控制
  setLoading(event: boolean)
  {
    this.notificationB18 = "";
    this.notificationS18 = "";
    this.loadingMsg = "檔案上傳中..."
    this.isLoading = event;
  }
}
