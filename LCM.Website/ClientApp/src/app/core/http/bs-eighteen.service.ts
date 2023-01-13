import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
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

  upload(uploadType:string, file: File): Observable<any> 
  {
    const url = `${environment.apiBaseUrl}/BsEighteen/upload`;
    const formData: FormData = new FormData();    
    formData.append('uploadType', uploadType);
    formData.append('postFile', file);   
    const options = this.generatePostOptions();  

    return this.httpClient.post(url, formData, { reportProgress: true, observe: 'events', withCredentials: true });
  }

  insertS18(filePath: string): Observable<IResultDto<string>> 
  {
    debugger;
    const url = `${environment.apiBaseUrl}/BsEighteen/InsertS18`;
    const options = this.generatePostOptions();

    return this.httpClient.post<IResultDto<string>>(url, {filePath: filePath}, options)
      // .pipe(
      //   tap((_) => this.log(''))
      //   //, map((result) => this.processResult(result))
      // );
  }

  // test(): Observable<any> {
  //   const url = `${environment.apiBaseUrl}/BsEighteen/Test`;
  //   const options = this.generatePostOptions();

  //   return this.httpClient
  //   .get<IResultDto<any>>(url, options)
  //   .pipe(
  //     tap((_) => console.log('execute BsEighteen.Test'))
  //     //, map((result) => this.processResult(result))
  //   );
  // }
}
