import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-new-upload',
  templateUrl: './new-upload.component.html',
  styleUrls: ['./new-upload.component.scss']
})

export class NewUploadComponent 
{
  @Input() public inputApiUrl: string = "";
  @Output() private outputUploadInfo: EventEmitter<UploadInfo>  = new EventEmitter();
  @ViewChild('myFileInput') myFileInput: any;

  selectedFiles?: FileList;
  progress:number = 0;
  fileName: string = '選擇檔案...';

  constructor(
    private httpClient: HttpClient
  ) { }

  selectFile(event: any): void
  {
    this.fileName = '';
    this.progress = 0;
    this.selectedFiles = undefined;

    if (event.target.files.length > 0)
    {
      if (event.target.files[0].type != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
      {
        this.selectedFiles = undefined;
        this.myFileInput.nativeElement.value = '';
        return
      }

      if (event.target.files[0].size > 100000000)
      {
        this.selectedFiles = undefined;
        this.myFileInput.nativeElement.value = '';
        return
      }

      this.selectedFiles = event.target.files;      
      this.fileName = event.target.files[0]?.name;
    }
  }

  upload()
  {
    this.progress = 0;

    if (this.selectedFiles) 
    {
      let uploadInfo: UploadInfo = {status: UploadType.Processing, uploadFileType: "", fileName: "", fullFilePath: ""};
      const file: File | null = this.selectedFiles.item(0);

      this.outputUploadInfo?.emit(uploadInfo);

      if (file)
      {
        const formData: FormData = new FormData();
        formData.append('postFile', file);
        
        this.httpClient.post(this.inputApiUrl, formData, { reportProgress: true, observe: 'events', withCredentials: true })
        .subscribe({
          next: async (event:any) => {
            if (event.type === HttpEventType.UploadProgress)
            {
              this.progress = Math.round(100 * event.loaded / event.total);
            }
            else if (event.type === HttpEventType.Response)
            {
              console.log(event); 
              uploadInfo = event.body.content;

              if (event.body.success == true)
              {
                //res = event.body;
                this.selectedFiles = undefined;
                this.myFileInput.nativeElement.value = '';
                uploadInfo.status = UploadType.Success;
              }
              else 
              {
                uploadInfo.status = UploadType.Fail;
              }              
            }
          },
          complete: () => {
            this.selectedFiles = undefined;
            this.myFileInput.nativeElement.value = '';
            this.outputUploadInfo?.emit(uploadInfo);
          },
          error: () => { 
            this.selectedFiles = undefined;
            this.myFileInput.nativeElement.value = '';
            this.outputUploadInfo?.emit(uploadInfo);
          },
        })
      }

      this.selectedFiles = undefined;
    }
  } 
}

export interface UploadInfo 
{    
    status?: number;
    uploadFileType?: string;
    fileName?: string;
    fullFilePath?: string;
    message?: string
}

export enum UploadType
{
    Processing = 0,
    Success = 1,
    Fail = 2
}
