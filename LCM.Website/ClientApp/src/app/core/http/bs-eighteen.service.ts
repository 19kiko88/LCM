import { FileInfo } from './../../shared/models/dto/request/file-info';
import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { map } from 'rxjs/internal/operators/map';
import { tap } from 'rxjs/internal/operators/tap';
import { IResultDto } from 'src/app/shared/models/dto/response/result-dto';
import { environment } from 'src/environments/environment';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root'
})

export class BsEighteenService extends BaseService 
{
  constructor(
    private httpClient: HttpClient
  ) 
  { 
    super(); 
  }

  //excel上傳後，大18資料寫入DB
  insertB18(data: FileInfo): Observable<IResultDto<string>> 
  {
    const url = `${environment.apiBaseUrl}/BsEighteen/InsertB18`;
    const options = this.generatePostOptions();

    return this.httpClient.post<IResultDto<string>>(url, data, options)
      // .pipe(
      //   tap((_) => this.log(''))
      //   //, map((result) => this.processResult(result))
      // );
  }

  //excel上傳後，小18資料寫入DB
  insertS18(data: FileInfo): Observable<IResultDto<string>>
  {
    const url = `${environment.apiBaseUrl}/BsEighteen/InsertS18`;
    const options = this.generatePostOptions();

    return this.httpClient.post<IResultDto<string>>(url, data, options)
  }

  //廠商提供excel上傳後，匯出PK報表
  exportPK(data: FileInfo): Observable<IResultDto<any>>
  {
    const url = `${environment.apiBaseUrl}/BsEighteen/ExportPkBs18`;
    const options:any = this.generatePostOptions();
    options.responseType = 'arraybuffer';

    return this.httpClient.post(url, data, options).pipe(
      map(data => {
        var enc = new TextDecoder("utf-8");
        var arr = new Uint8Array(data);  

        let res:IResultDto<any> = {id:'0', success : true, message: '', content: null};
        try
        {//轉成IResultDto沒有失敗代表excel產生異常，經過後端ExceptionFilter，取得Exception
          res = JSON.parse(enc.decode(arr));
        }
        catch
        {//轉IResultDto失敗代表excel產生正常，沒有經過後端ExceptionFilter，開始下載excel
          this.downloadFile(
            'PK_RESULT.xlsx',
            data,
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
          )          
        }
        return res;
      })
    )
  }

    //手動結案
    manualClose(data: FileInfo): Observable<IResultDto<number>>
    {
      const url = `${environment.apiBaseUrl}/BsEighteen/ManualClose`;
      const options = this.generatePostOptions();
  
      return this.httpClient.post<IResultDto<number>>(url, data, options)
    }

  private downloadFile(name: string, data: any, type: string)
  {     
    const blob = new Blob([data], { type: type });
    const url = window.URL.createObjectURL(blob);
    var link = document.createElement('a');
    link.href = url;
    link.download = name;
    link.click();
    link.remove();
  }

}
