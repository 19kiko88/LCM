import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedNgMaterialModule } from './shared-ngmaterial/shared-ngmaterial.module';
import { MenuItemComponent } from './components/menu-item/menu-item.component'
import { RouterModule } from '@angular/router';
import { SharedNgbootstrapModule } from './shared-ngbootstrap/shared-ngbootstrap.module';
import { UploadComponent } from './components/upload/upload.component';
import { LoadingComponent } from './components/loading/loading.component';

@NgModule({
  declarations: [MenuItemComponent, UploadComponent, LoadingComponent, LoadingComponent],
  imports: [
    CommonModule,
    SharedNgMaterialModule,
    SharedNgbootstrapModule,
    RouterModule
  ],
  exports:[MenuItemComponent, UploadComponent, LoadingComponent]
})
export class SharedModule { }
