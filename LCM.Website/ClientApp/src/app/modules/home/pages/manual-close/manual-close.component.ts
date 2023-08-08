import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BsEighteenService } from 'src/app/core/http/bs-eighteen.service';
import { SweetAlertService } from 'src/app/core/service/sweet-alert.service';
import { UploadInfo, UploadType } from 'src/app/shared/components/new-upload/new-upload.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-manual-close',
  templateUrl: './manual-close.component.html',
  styleUrls: ['./manual-close.component.scss']
})
export class ManualCloseComponent implements OnInit {

  isLoading: boolean = false;
  loadingMsg: string = "";
  uploadApiURL: string = "";
  updateResult: string = "";

  constructor(
    private _bsEighteenService: BsEighteenService,
    private _swlService: SweetAlertService
  ){  }

  ngOnInit()
  {
    this.uploadApiURL = `${environment.apiBaseUrl}/BsEighteen/Upload`;
  }

   setUploadInfo(data: UploadInfo)
   {     
     if(data.status == UploadType.Processing)
     {
      this.loadingMsg = '資料上傳中...';
       this.isLoading = true;
     }
     else if (data.status == UploadType.Success)
     {    
      this.loadingMsg = '手動更新結案狀態處理中...';

      this._bsEighteenService.manualClose(data).subscribe(res => {
        if (res.success)
        {
          this._swlService.showSwal('', '手動更新結案狀態成功.', 'info');
          this.updateResult = `更新筆數：${res.content}`;
        }
        else 
        {
          this._swlService.showSwal('', `手動更新結案狀態異常，請聯絡CAE Team.<br\>${res.message}`, 'error');
        }

        this.isLoading = false;
      })
     }
     else if(data.status == UploadType.Fail)
     {
       this._swlService.showSwal('', `${data.fileName}上傳失敗,請聯絡CAE Team。${data.message}`, 'error');
       this.isLoading = false;
     }
   }   

}
