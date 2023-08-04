import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedNgMaterialModule } from './shared-ngmaterial/shared-ngmaterial.module';
import { MenuItemComponent } from './components/menu-item/menu-item.component'
import { RouterModule } from '@angular/router';
import { SharedNgbootstrapModule } from './shared-ngbootstrap/shared-ngbootstrap.module';
import { LoadingComponent } from './components/loading/loading.component';
import { NewUploadComponent } from './components/new-upload/new-upload.component';
import { UploadComponent } from './components/upload/upload.component';
import { MatCardModule } from '@angular/material/card';
import { SelectComponent } from './components/select/select.component';

@NgModule({
  declarations: [MenuItemComponent, UploadComponent, LoadingComponent, LoadingComponent, NewUploadComponent, SelectComponent],
  imports: [
    CommonModule,
    SharedNgMaterialModule,
    SharedNgbootstrapModule,
    RouterModule,
    FormsModule
  ],
  exports:[MenuItemComponent, UploadComponent, NewUploadComponent, LoadingComponent, SelectComponent, MatCardModule]
})
export class SharedModule { }
