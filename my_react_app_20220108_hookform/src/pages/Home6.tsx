import React from 'react';
import Select from "react-select";
import { useForm, Controller, SubmitHandler } from "react-hook-form";
import {List, ListItem, TextField, Checkbox} from '@mui/material';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

interface IFormInputs {
  TextField: string
  MyCheckbox: boolean
}

const Home6 = () => {
  const { handleSubmit, control, reset } = useForm<IFormInputs>();
  const onSubmit: SubmitHandler<IFormInputs> = (data) => {
    console.log(data);
  }
  
  return (
    <ResponsiveDrawer>
      <div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <List>
            <ListItem>
              <label>MyCheckbox</label>
              <Controller
                name="MyCheckbox"
                control={control}
                defaultValue={false}
                rules={{ required: true }}
                render={({ field }) => <Checkbox {...field} />}
              />
            </ListItem><ListItem>
              <input type="submit" />
            </ListItem>
          </List>
        </form>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home6;